using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;

        public HomeController(KitapAlimSatimDbContext kitapAlimSatimDbContext)
        {
            _kitapAlimSatimDbContext = kitapAlimSatimDbContext;
        }

        public IActionResult Index()
        {
            // aktif olan ürünleri oluşturma tarihine göre azalan şekilde getir. 
            List<Product> products = _kitapAlimSatimDbContext.Product.Where(e => e.IsActive == true).OrderByDescending(e => e.CreatedAt).Take(12).ToList();
            List<Book> books = _kitapAlimSatimDbContext.Book.ToList();

            // modele uygun hale getiren sorgu
            var model = from p in products
                        join b in books on p.BookId equals b.Id into table1
                        from b in table1.ToList()
                        select new ProductModel
                        {
                            Book = b,
                            Product = p
                        };
            return View(model);
        }

        [HttpPost]
        public IActionResult CultureManager(string culture, string returnUrl)
        {
            // Dil fonksiyonu
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    // 10 yıl cookie süresi
                    Expires = DateTimeOffset.Now.AddYears(10)
                });

            // Oturum açıksa kullanıcı dilini değiştirme
            string login = HttpContext.Session.GetString("Login");
            if(login != null)
            {
                var user = _kitapAlimSatimDbContext.User.Where(e => e.Email.Equals(login)).SingleOrDefault();
                
                if(user != null)
                {
                    user.Language = culture;
                    _kitapAlimSatimDbContext.SaveChanges();
                }
            }
            
            return Redirect(returnUrl);
        }

        public IActionResult Search()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Search(string search)
        {
            // aktif olan ürünleri oluşturma tarihine göre azalan şekilde getir. 
            List<Product> products = _kitapAlimSatimDbContext.Product.Where(e => e.IsActive == true).OrderByDescending(e => e.CreatedAt).ToList();
            List<Book> books = _kitapAlimSatimDbContext.Book.Where(
                e => e.Name.ToLower().Contains(search.ToLower())
                || e.Author.ToLower().Contains(search.ToLower())
                || e.Publisher.ToLower().Contains(search.ToLower())).Take(12).ToList();

            // modele uygun hale getiren sorgu
            var model = from p in products
                        join b in books on p.BookId equals b.Id into table1
                        from b in table1.ToList()
                        select new ProductModel
                        {
                            Book = b,
                            Product = p
                        };
            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
