using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Entities
{
    class User : BaseEntity
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public bool IsAdmin { get; set; } = false;

        public string UserSettings { get; set; }
    }
}
