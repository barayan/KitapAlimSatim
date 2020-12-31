using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;
        public User user;

        public ProductController(KitapAlimSatimDbContext kitapAlimSatimDbContext)
        {
            _kitapAlimSatimDbContext = kitapAlimSatimDbContext;
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

        public IActionResult Index(int productId)
        {
            Product product = _kitapAlimSatimDbContext.Product.Find(productId);
            ProductModel model = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(product));
            model.Book = _kitapAlimSatimDbContext.Book.Find(model.BookId);
            model.User = _kitapAlimSatimDbContext.User.Find(model.UserId);
            return View(model);
        }

        public IActionResult Add()
        {
            List<Book> model = _kitapAlimSatimDbContext.Book.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(int book, int price, int stock)
        {
            GetUser();
            List<Book> model = _kitapAlimSatimDbContext.Book.ToList();

            bool bookExist = _kitapAlimSatimDbContext.Book.Find(book) != null;
            if (bookExist && price > 0 && stock >= 1 && user != null)
            {
                _kitapAlimSatimDbContext.Product.Add(new Product
                {
                    BookId = book,
                    Price = price,
                    Stock = stock,
                    UserId = user.Id
                });
                _kitapAlimSatimDbContext.SaveChanges();
                ViewData["ProductSuccess"] = "Ürün kaydedildi.";
            }
            else ViewData["ProductError"] = "Hata meydana geldi.";
            return View(model);
        }
    }
}
