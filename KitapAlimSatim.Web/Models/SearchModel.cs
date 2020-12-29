using KitapAlimSatim.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class SearchModel
    {
        public Product product { get; set; }
        public Book book { get; set; }
    }
}
