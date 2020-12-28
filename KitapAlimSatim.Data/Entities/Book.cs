using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    public class Book : BaseEntity
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string FileName { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }
    }
}
