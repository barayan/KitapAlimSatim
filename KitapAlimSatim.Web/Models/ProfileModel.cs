using KitapAlimSatim.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class ProfileModel : User
    {
        public string Request { get; set; }

        public string Action { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string Table { get; set; }
    }
}
