using KitapAlimSatim.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class BookModel : Book
    {
        public IFormFile Image { get; set; }
    }
}
