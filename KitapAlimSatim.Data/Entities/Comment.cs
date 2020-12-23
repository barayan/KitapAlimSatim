using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    class Comment : BaseEntity
    {
        public string Message { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }
    }
}
