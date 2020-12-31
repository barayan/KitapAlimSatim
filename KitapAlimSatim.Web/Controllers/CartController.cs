﻿using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;
        private readonly IHtmlLocalizer<CartController> _localizer;
        public User user;

        public CartController(KitapAlimSatimDbContext kitapAlimSatimDbContext, IHtmlLocalizer<CartController> localizer)
        {
            _kitapAlimSatimDbContext = kitapAlimSatimDbContext;
            _localizer = localizer;
        }

        private void GetUser()
        {
            // Oturum açıldıysa user nesnesini dolduruyoruz
            string login = HttpContext.Session.GetString("Login");
            if (login != null)
            {
                user = _kitapAlimSatimDbContext.User.Where(e => e.Email.Equals(login)).SingleOrDefault();
            }
        }

        public IActionResult Index()
        {
            var cookie = HttpContext.Request.Cookies["ProductCart"];
            if (cookie == null) cookie = "[]";
            var items = JsonConvert.DeserializeObject<int[]>(Uri.UnescapeDataString(cookie));
            var products = _kitapAlimSatimDbContext.Product.Where(e => items.Contains(e.Id)).ToList();
            List<CartModel> model = new List<CartModel>();
            double total = 0;
            foreach (var item in products)
            {
                var pModel = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(item));
                pModel.User = _kitapAlimSatimDbContext.User.Find(pModel.UserId);
                pModel.Book = _kitapAlimSatimDbContext.Book.Find(pModel.BookId);

                model.Add(new CartModel
                {
                    Product = pModel
                });
                total += pModel.Price;
            }
            // format biçimini Türk lirasına uygun hale getiriyoruz.
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ",";
            customCulture.NumberFormat.NumberGroupSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            ViewData["SubTotalDisplay"] = String.Format("{0:#,0.00}", total);
            ViewData["SubTotal"] = total;
            return View(model);
        }

        public IActionResult Checkout()
        {
            GetUser();
            CheckoutModel model = new CheckoutModel();
            if(user != null)
            {
                var cname = "ProductCart";
                var cookie = HttpContext.Request.Cookies[cname];
                if (cookie == null) cookie = "[]";
                var items = JsonConvert.DeserializeObject<int[]>(Uri.UnescapeDataString(cookie));
                var products = _kitapAlimSatimDbContext.Product.Where(e => items.Contains(e.Id)).ToList();
                double subtotal = 0;
                List<ProductModel> orderItems = new List<ProductModel>();
                foreach (var item in products)
                {
                    subtotal += item.Price;
                    var pModel = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(item));
                    orderItems.Add(pModel);
                }
                var order = new Order
                {
                    UserId = user.Id,
                    SubTotal = subtotal,
                    OrderItems = JsonConvert.SerializeObject(orderItems)
                };
                _kitapAlimSatimDbContext.Order.Add(order);
                _kitapAlimSatimDbContext.SaveChanges();
                Response.Cookies.Append(cname, "",
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(-1)
                    });
                model.Success = true;
                model.Message = _localizer["CheckoutSuccess"].Value;
            }else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
    }
}
