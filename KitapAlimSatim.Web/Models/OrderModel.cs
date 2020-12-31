using KitapAlimSatim.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class OrderModel : Order
    {
        public User User { get; set; }

        public List<ProductModel> OrderItemList { get; set; }
    }
}
