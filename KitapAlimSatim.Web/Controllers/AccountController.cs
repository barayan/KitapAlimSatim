using KitapAlimSatim.Data;
using KitapAlimSatim.Data.Entities;
using KitapAlimSatim.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;
        private readonly IHtmlLocalizer<AccountController> _localizer;
        public User user;

        public AccountController(KitapAlimSatimDbContext kitapAlimSatimDbContext, IHtmlLocalizer<AccountController> localizer)
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
            GetUser();
            if (user == null)
            {
                HttpContext.Session.Remove("Login");
                return Redirect("/");
            }
            return View(new List<User> { user });
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
                    User user = users.First();
                    HttpContext.Session.SetString("Login", user.Email);
                    Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(user.Language)),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddYears(10)
                        });
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

        [HttpPost]
        public ContentResult Get(ProfileModel model)
        {
            GetUser();
            string data = "false", error = "";
            bool success = true;
            if(user != null)
            {
                switch (model.Request)
                {
                    case "ayarlar":
                        success = false;
                        switch (model.Action)
                        {
                            case "PersonalData":
                                if(model.Name == null)
                                {
                                    error = _localizer["ErrorNameNull"].Value;
                                }else
                                {
                                    user.Name = model.Name;
                                    user.Address = model.Address;
                                    success = true;
                                }
                                break;
                            case "ChangePassword":
                                if(model.Password != user.Password)
                                {
                                    error = _localizer["PasswordIncorrect"].Value;
                                }else if(model.NewPassword != model.ConfirmNewPassword || model.NewPassword == null)
                                {
                                    error = _localizer["PasswordsNotMatch"].Value;
                                }else
                                {
                                    user.Password = model.NewPassword;
                                    success = true;
                                }
                                break;
                            default:
                                break;
                        }
                        _kitapAlimSatimDbContext.SaveChanges();
                        break;
                    case "siparislerim":
                        var orders = _kitapAlimSatimDbContext.Order.Where(e => e.UserId == user.Id).OrderByDescending(e => e.CreatedAt).ToList();
                        List<OrderModel> list = new List<OrderModel>();
                        foreach(Order item in orders)
                        {
                            var user = _kitapAlimSatimDbContext.User.Find(item.UserId);
                            if (user == null || item.OrderItems == null) continue;
                            var orderItems = JsonConvert.DeserializeObject<List<ProductModel>>(item.OrderItems);
                            // içini doldurma
                            foreach(var oitem in orderItems)
                            {
                                oitem.User = _kitapAlimSatimDbContext.User.Find(oitem.UserId);
                                oitem.Book = _kitapAlimSatimDbContext.Book.Find(oitem.BookId);
                            }
                            var orderItem = JsonConvert.DeserializeObject<OrderModel>(JsonConvert.SerializeObject(item));
                            orderItem.OrderItemList = orderItems;
                            orderItem.User = user;
                            list.Add(orderItem);
                        }
                        data = JsonConvert.SerializeObject(list);
                        break;
                    case "satislarim":
                        var products = _kitapAlimSatimDbContext.Product
                                .Join(_kitapAlimSatimDbContext.Book, product => product.BookId, book => book.Id, (product, book) => new { product, book })
                                .Where(e => e.product.UserId == user.Id).OrderByDescending(e => e.product.CreatedAt).ToList();
                        data = JsonConvert.SerializeObject(products);
                        break;
                    default:
                        break;
                }
                // Admin istekleri
                if(user.IsAdmin == true)
                {
                    switch (model.Request)
                    {
                        case "kullanicilar":
                            var users = _kitapAlimSatimDbContext.User.OrderByDescending(e => e.CreatedAt).ToList();
                            data = JsonConvert.SerializeObject(users);
                            break;
                        case "kitaplar":
                            var books = _kitapAlimSatimDbContext.Book.OrderByDescending(e => e.CreatedAt).ToList();
                            data = JsonConvert.SerializeObject(books);
                            break;
                        case "urunler":
                            var products = _kitapAlimSatimDbContext.Product
                                .Join(_kitapAlimSatimDbContext.Book, product => product.BookId, book => book.Id, (product, book) => new { product, book })
                                .Join(_kitapAlimSatimDbContext.User, product => product.product.UserId, user => user.Id, (product, user) => new { product, user})
                                .OrderByDescending(e => e.product.product.CreatedAt).ToList();
                            data = JsonConvert.SerializeObject(products);
                            break;
                        case "siparisler":
                            var orders = _kitapAlimSatimDbContext.Order.OrderByDescending(e => e.CreatedAt).ToList();
                            List<OrderModel> list = new List<OrderModel>();
                            foreach (Order item in orders)
                            {
                                var user = _kitapAlimSatimDbContext.User.Find(item.UserId);
                                if (user == null || item.OrderItems == null) continue;
                                var orderItems = JsonConvert.DeserializeObject<List<ProductModel>>(item.OrderItems);
                                var orderItem = JsonConvert.DeserializeObject<OrderModel>(JsonConvert.SerializeObject(item));
                                orderItem.OrderItemList = orderItems;
                                orderItem.User = user;
                                list.Add(orderItem);
                            }
                            data = JsonConvert.SerializeObject(list);
                            break;
                        case "yorumlar":
                            var comments = _kitapAlimSatimDbContext.Comment
                                .Join(_kitapAlimSatimDbContext.User, comment => comment.UserId, user => user.Id, (comment, user) => new { comment, user })
                                .OrderByDescending(e => e.comment.CreatedAt).ToList();
                            List<CommentModel> commentList = new List<CommentModel>();
                            foreach(var item in comments)
                            {
                                var commentItem = JsonConvert.DeserializeObject<CommentModel>(JsonConvert.SerializeObject(item.comment));
                                commentItem.User = item.user;
                                var product = _kitapAlimSatimDbContext.Product.Find(item.comment.ProductId);
                                commentItem.Product = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(product));
                                commentItem.Product.Book = _kitapAlimSatimDbContext.Book.Find(commentItem.Product.BookId);
                                commentList.Add(commentItem);
                            }
                            data = JsonConvert.SerializeObject(comments);
                            break;
                        default:
                            break;
                    }
                }
            }
            data = "{\"success\": " + (success ? "true" : "false") + ", \"data\": " + data + ", \"error\": \"" + error + "\"}";
            return Content(data, "application/json");
        }
    }
}
