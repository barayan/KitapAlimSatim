using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class CheckoutModel
    {
        public bool Success { get; set; } = false;

        public string Error { get; set; }

        public string Message { get; set; }
    }
}
