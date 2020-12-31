using KitapAlimSatim.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class ProductModel : Product
    {
        public Product Product { get; set; }
        public Book Book { get; set; }

        public User User { get; set; }
    }
}
