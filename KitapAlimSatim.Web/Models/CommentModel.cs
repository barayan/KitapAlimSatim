using KitapAlimSatim.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class CommentModel : Comment
    {
        public User User { get; set; }

        public ProductModel Product { get; set; }
    }
}
