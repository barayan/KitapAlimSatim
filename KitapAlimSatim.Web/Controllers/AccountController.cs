using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;
        private readonly IHtmlLocalizer<AccountController> _localizer;

        public AccountController(KitapAlimSatimDbContext kitapAlimSatimDbContext, IHtmlLocalizer<AccountController> localizer)
        {
            _kitapAlimSatimDbContext = kitapAlimSatimDbContext;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Login");
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if(email != null && password != null)
            {
                List<User> users = _kitapAlimSatimDbContext.User.Where(
                e => e.Email.ToLower().Equals(email.ToLower())
                && e.Password.Equals(password)
                ).ToList();

                if (users.Count > 0)
                {
                    HttpContext.Session.SetString("Login", users.First().Email);
                    return Redirect("/");
                }
            }
            ModelState.AddModelError("LoginError", _localizer["InvalidCredentials"].Value);
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel registerModel)
        {
            string error = null;
            if(ModelState.IsValid)
            {
                bool exists = _kitapAlimSatimDbContext.User.Where(e => e.Email.ToLower().Equals(registerModel.email.ToLower())).Any();

                if(exists)
                {
                    error = _localizer["EmailIsTaken"].Value;
                }
                else if (registerModel.password != registerModel.confirmPassword)
                {
                    error = _localizer["PasswordsNotMatch"].Value;
                }
                else
                {
                    var user = new User
                    {
                        Email = registerModel.email.ToLower(),
                        Password = registerModel.password,
                        Name = registerModel.name
                    };
                    _kitapAlimSatimDbContext.User.Add(user);
                    _kitapAlimSatimDbContext.SaveChanges();
                    ViewData["RegisterState"] = _localizer["RegisterSuccess"].Value;
                }
            }
            else
            {
                error = _localizer["RegisterGeneralError"].Value;
            }
            if(error != null) ModelState.AddModelError("RegisterError", error);
            return View("Login");
        }

    }
}
