using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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
            if(ViewData["kitaplar"] == null) ViewData["kitaplar"] = _kitapAlimSatimDbContext.Set<Book>().OrderByDescending(e => e.CreatedAt).Take(12).ToList();
            return View();
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

        [HttpPost]
        public IActionResult Search(string search)
        {
            ViewData["kitaplar"] = _kitapAlimSatimDbContext.Set<Book>().Take(1).ToList();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
