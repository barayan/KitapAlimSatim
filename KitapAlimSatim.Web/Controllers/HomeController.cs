using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
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
            List<Book> kitaplar = _kitapAlimSatimDbContext.Set<Book>().OrderByDescending(e => e.CreatedAt).Take(12).ToList();
            ViewData["kitaplar"] = kitaplar;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
