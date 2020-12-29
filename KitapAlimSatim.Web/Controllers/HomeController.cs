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
            List<Product> products = _kitapAlimSatimDbContext.Product.OrderByDescending(e => e.CreatedAt).Take(12).ToList();
            List<Book> books = _kitapAlimSatimDbContext.Book.ToList();
            var model = from p in products
                        join b in books on p.BookId equals b.Id into table1
                        from b in table1.ToList()
                        select new SearchModel
                        {
                            book = b,
                            product = p
                        };
            return View(model);
        }

        [HttpPost]
        public IActionResult CultureManager(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddYears(10)
                });

            return Redirect(returnUrl);
        }

        public IActionResult Search()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Search(string search)
        {
            List<Product> products = _kitapAlimSatimDbContext.Product.OrderByDescending(e => e.CreatedAt).ToList();
            List<Book> books = _kitapAlimSatimDbContext.Book.Where(
                e => e.Name.ToLower().Contains(search.ToLower())
                || e.Author.ToLower().Contains(search.ToLower())
                || e.Publisher.ToLower().Contains(search.ToLower())).Take(12).ToList();

            var model = from p in products
                        join b in books on p.BookId equals b.Id into table1
                        from b in table1.ToList()
                        select new SearchModel
                        {
                            book = b,
                            product = p
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
