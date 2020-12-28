using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }

        public double SubTotal { get; set; }

        public string OrderItems { get; set; }
    }
}
