using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
