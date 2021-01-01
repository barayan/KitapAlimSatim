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
using System.IO;
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
            // Oturum açıldıysa daha sonra kullanmak için user nesnesini dolduruyoruz
            string login = HttpContext.Session.GetString("Login");
            if (login != null)
            {
                user = _kitapAlimSatimDbContext.User.Where(e => e.Email.Equals(login)).SingleOrDefault();
            }
        }

        private string GenerateRandomString(int lenght = 10)
        {
            // Aşağıdaki karakterlerden verilen değer kadar rastgele karakter üreten fonksiyon. 
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[lenght];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public IActionResult Index()
        {
            GetUser();
            if (user == null)
            {
                // kullanıcı oturum açmadıysa anasayfaya yönlendir
                HttpContext.Session.Remove("Login");
                return Redirect("/");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            // GET ile erişimde login sayfasına yönlendir
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
            // Oturum açma fonksiyonu
            if(email != null && password != null)
            {
                List<User> users = _kitapAlimSatimDbContext.User.Where(
                e => e.Email.ToLower().Equals(email.ToLower())
                && e.Password.Equals(password)
                ).ToList();

                // eğer eşleşme varsa oturum aç ve anasayfaya yönlendir
                if (users.Count > 0)
                {
                    User user = users.First();
                    HttpContext.Session.SetString("Login", user.Email);
                    // dili kullanıcının diline göre ayarlıyoruz.
                    Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(user.Language)),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddYears(10)
                        });
                    return Redirect("/");
                }
            }
            // yönlendirme olmadıysa hata göster
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

                // e-posta adresi alındı mı
                if(exists)
                {
                    error = _localizer["EmailIsTaken"].Value;
                }
                // parolalar uyuşuyor mu 
                else if (registerModel.password != registerModel.confirmPassword)
                {
                    error = _localizer["PasswordsNotMatch"].Value;
                }
                else
                {
                    // hata yok. kaydet
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
                // model geçersiz.
                error = _localizer["RegisterGeneralError"].Value;
            }
            if(error != null) ModelState.AddModelError("RegisterError", error);
            return View("Login");
        }

        [HttpPost]
        public ContentResult Get(ProfileModel model)
        {
            // Profil sayfasındaki ajax sorguları için kullanılan fonksiyon
            GetUser();
            string data = "false", error = "";
            bool success = true;
            if(user != null)
            {
                // Genel istekler
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
                    case "delete":
                        if(user.IsAdmin == true)
                        {
                            switch (model.Table)
                            {
                                case "kullanicilar":
                                    var user = _kitapAlimSatimDbContext.User.Find(model.Id);
                                    _kitapAlimSatimDbContext.User.Remove(user);
                                    // cascade
                                    var userComments = _kitapAlimSatimDbContext.Comment.Where(e => e.UserId == model.Id).ToList();
                                    _kitapAlimSatimDbContext.Comment.RemoveRange(userComments);
                                    //
                                    var userProducts = _kitapAlimSatimDbContext.Product.Where(e => e.UserId == model.Id).ToList();
                                    _kitapAlimSatimDbContext.Product.RemoveRange(userProducts);
                                    //
                                    var userOrders = _kitapAlimSatimDbContext.Order.Where(e => e.UserId == model.Id).ToList();
                                    _kitapAlimSatimDbContext.Order.RemoveRange(userOrders);
                                    break;
                                case "kitaplar":
                                    var book = _kitapAlimSatimDbContext.Book.Find(model.Id);
                                    _kitapAlimSatimDbContext.Book.Remove(book);
                                    // cascade
                                    var bookProducts = _kitapAlimSatimDbContext.Product.Where(e => e.BookId == model.Id).ToList();
                                    _kitapAlimSatimDbContext.Product.RemoveRange(bookProducts);
                                    break;
                                case "siparisler":
                                    var order = _kitapAlimSatimDbContext.Order.Find(model.Id);
                                    _kitapAlimSatimDbContext.Order.Remove(order);
                                    break;
                                case "yorumlar":
                                    var comment = _kitapAlimSatimDbContext.Comment.Find(model.Id);
                                    _kitapAlimSatimDbContext.Comment.Remove(comment);
                                    break;
                                default:
                                    break;
                            }
                        }
                        switch (model.Table)
                        {
                            case "urunler":
                            case "satislarim":
                                var product = _kitapAlimSatimDbContext.Product.Find(model.Id);
                                if(user.IsAdmin == true || product.UserId == user.Id)
                                {
                                    _kitapAlimSatimDbContext.Product.Remove(product);
                                    // cascade
                                    var productComments = _kitapAlimSatimDbContext.Comment.Where(e => e.ProductId == model.Id).ToList();
                                    _kitapAlimSatimDbContext.Comment.RemoveRange(productComments);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "active":
                        switch (model.Table)
                        {
                            case "urunler":
                            case "satislarim":
                                var product = _kitapAlimSatimDbContext.Product.Find(model.Id);
                                if (user.IsAdmin == true || product.UserId == user.Id)
                                {
                                    product.IsActive = !product.IsActive;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "admin":
                        var u = _kitapAlimSatimDbContext.User.Find(model.Id);
                        if (u != null && user.IsAdmin == true)
                        {
                            u.IsAdmin = !u.IsAdmin;
                        }
                        break;
                    default:
                        break;
                }
                // yapılan değişiklikleri kaydet
                _kitapAlimSatimDbContext.SaveChanges();
                // Admin istekleri
                if (user.IsAdmin == true)
                {
                    switch (model.Request)
                    {
                        case "kullanicilar":
                            var users = _kitapAlimSatimDbContext.User.OrderByDescending(e => e.Id).ToList();
                            data = JsonConvert.SerializeObject(users);
                            break;
                        case "kitaplar":
                            var books = _kitapAlimSatimDbContext.Book.OrderByDescending(e => e.Id).ToList();
                            data = JsonConvert.SerializeObject(books);
                            break;
                        case "urunler":
                            // ürünün kitabını ve ürünü satan kullanıcıyı join ederek birleştiriyoruz.
                            var products = _kitapAlimSatimDbContext.Product
                                .Join(_kitapAlimSatimDbContext.Book, product => product.BookId, book => book.Id, (product, book) => new { product, book })
                                .Join(_kitapAlimSatimDbContext.User, product => product.product.UserId, user => user.Id, (product, user) => new { product, user})
                                .OrderByDescending(e => e.product.product.CreatedAt).ToList();
                            data = JsonConvert.SerializeObject(products);
                            break;
                        case "siparisler":
                            // modele göre siparişleri alıyoruz.
                            var orders = _kitapAlimSatimDbContext.Order.OrderByDescending(e => e.CreatedAt).ToList();
                            List<OrderModel> list = new List<OrderModel>();
                            foreach (Order item in orders)
                            {
                                var user = _kitapAlimSatimDbContext.User.Find(item.UserId);
                                if (user == null || item.OrderItems == null) continue;
                                var orderItems = JsonConvert.DeserializeObject<List<ProductModel>>(item.OrderItems);
                                // içini doldurma
                                foreach (var oitem in orderItems)
                                {
                                    // ürünü satan satıcıyı ve kitabı alıyoruz.
                                    oitem.User = _kitapAlimSatimDbContext.User.Find(oitem.UserId);
                                    oitem.Book = _kitapAlimSatimDbContext.Book.Find(oitem.BookId);
                                }
                                // modeli ebeveyn sınıf ile dolduruyoruz.
                                var orderItem = JsonConvert.DeserializeObject<OrderModel>(JsonConvert.SerializeObject(item));
                                orderItem.OrderItemList = orderItems;
                                // siparişi veren kullanıcı
                                orderItem.User = _kitapAlimSatimDbContext.User.Find(item.UserId);
                                list.Add(orderItem);
                            }
                            data = JsonConvert.SerializeObject(list);
                            break;
                        case "yorumlar":
                            // join işlemi ile kullanıcı tablosunu da alıyoruz.
                            var comments = _kitapAlimSatimDbContext.Comment
                                .Join(_kitapAlimSatimDbContext.User, comment => comment.UserId, user => user.Id, (comment, user) => new { comment, user })
                                .OrderByDescending(e => e.comment.CreatedAt).ToList();
                            // modele göre verileri dolduruyoruz
                            List<CommentModel> commentList = new List<CommentModel>();
                            foreach(var item in comments)
                            {
                                // modeli ebeveyn sınıf ile dolduruyoruz.
                                var commentItem = JsonConvert.DeserializeObject<CommentModel>(JsonConvert.SerializeObject(item.comment));
                                commentItem.User = item.user;
                                var product = _kitapAlimSatimDbContext.Product.Find(item.comment.ProductId);
                                // modeli ebeveyn sınıf ile dolduruyoruz.
                                commentItem.Product = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(product));
                                commentItem.Product.Book = _kitapAlimSatimDbContext.Book.Find(commentItem.Product.BookId);
                                commentList.Add(commentItem);
                            }
                            data = JsonConvert.SerializeObject(commentList);
                            break;
                        default:
                            break;
                    }
                }
            }
            // json oluşturup yolluyoruz. 
            data = "{\"success\": " + (success ? "true" : "false") + ", \"data\": " + data + ", \"error\": \"" + error + "\"}";
            return Content(data, "application/json");
        }


        [HttpPost]
        public IActionResult Post(BookModel model)
        {
            GetUser();
            // Resim yoksa ve kullanıcı admin değilse işlem yapmıyor
            if (model.Image != null && user != null && user.IsAdmin == true)
            {
                // resimin uzantısını ve dosya adını alıyoruz.
                string extension = Path.GetExtension(model.Image.FileName);
                string ImageName = Guid.NewGuid().ToString(), SavePath;

                string random = ImageName + extension;
                while(true)
                {
                    // kaydetmek için dosya yolunu al
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/media/image", random);
                    // Bu isimle dosya yoksa devam et
                    if (!System.IO.File.Exists(SavePath)) break;
                    // eğer varsa rastgele dosya ismi oluştur
                    random = GenerateRandomString(ImageName.Length) + extension;
                }

                // veritabanına ekle
                _kitapAlimSatimDbContext.Book.Add(new Book
                {
                    Author = model.Author,
                    Description = model.Description,
                    FileName = random,
                    Name = model.Name,
                    Publisher = model.Publisher
                });

                // dosyayı oluştur
                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    // kopyala
                    model.Image.CopyTo(stream);
                    // veritabanına kaydet
                    _kitapAlimSatimDbContext.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
