using KitapAlimSatim.Data;
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
    public class ProductController : Controller
    {
        private readonly KitapAlimSatimDbContext _kitapAlimSatimDbContext;
        private readonly IHtmlLocalizer<ProductController> _localizer;
        public User user;

        public ProductController(KitapAlimSatimDbContext kitapAlimSatimDbContext, IHtmlLocalizer<ProductController> localizer)
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

        public IActionResult Index(int productId)
        {
            Product product = _kitapAlimSatimDbContext.Product.Find(productId);
            // modeli ebeveyn sınıf ile dolduruyoruz.
            ProductModel model = JsonConvert.DeserializeObject<ProductModel>(JsonConvert.SerializeObject(product));
            // kullanıcı ve kitap bilgilerini alıyoruz.
            model.Book = _kitapAlimSatimDbContext.Book.Find(model.BookId);
            model.User = _kitapAlimSatimDbContext.User.Find(model.UserId);
            // ürünün yorumlarını modele göre alıyoruz
            var comments = _kitapAlimSatimDbContext.Comment.Where(e => e.ProductId == productId).ToList();
            // modeli ebeveyn sınıf ile dolduruyoruz.
            var commentModel = JsonConvert.DeserializeObject<List<CommentModel>>(JsonConvert.SerializeObject(comments));
            foreach(var item in commentModel)
            {
                item.User = _kitapAlimSatimDbContext.User.Find(item.UserId);
            }
            ViewData["comments"] = commentModel;
            return View(model);
        }

        public IActionResult Add()
        {
            GetUser();
            if (user == null) return RedirectToAction("Login", "Account");
            // Kitapları gönderiyoruz.
            List<Book> model = _kitapAlimSatimDbContext.Book.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(int book, double price)
        {
            // ürün ekleme fonskiyonu
            GetUser();
            List<Book> model = _kitapAlimSatimDbContext.Book.ToList();

            // kitap kayıtlıysa ve oturum açıldıysa devam et
            bool bookExist = _kitapAlimSatimDbContext.Book.Find(book) != null;
            if (bookExist && price > 0 && user != null)
            {
                _kitapAlimSatimDbContext.Product.Add(new Product
                {
                    BookId = book,
                    Price = price,
                    Stock = 1,
                    UserId = user.Id
                });
                _kitapAlimSatimDbContext.SaveChanges();
                ViewData["ProductSuccess"] = _localizer["ProductSuccess"].Value;
            }
            else ViewData["ProductError"] = _localizer["ProductError"].Value;
            return View(model);
        }

        [HttpPost]
        public IActionResult Comment(string comment, int productId)
        {
            // yorum ekleme fonksiyonu
            GetUser();
            // oturum açıksa veya yorum boş değilse devam et
            if(user != null && !string.IsNullOrEmpty(comment))
            {
                _kitapAlimSatimDbContext.Comment.Add(new Comment
                {
                    Message = comment,
                    ProductId = productId,
                    UserId = user.Id
                });
                _kitapAlimSatimDbContext.SaveChanges();
            }
            // ürüne geri dön
            return RedirectToAction("Index", new { productId = productId });
        }
    }
}
