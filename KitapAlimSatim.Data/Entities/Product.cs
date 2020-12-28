using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    public class Product : BaseEntity
    {
        public int BookId { get; set; }

        public int UserId { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }

        public bool? IsActive { get; set; }
    }
}
