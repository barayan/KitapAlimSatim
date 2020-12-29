using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KitapAlimSatim.Web.Models
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }

        [Required]
        [MaxLength(300)]
        public string password { get; set; }

        [Required]
        public string confirmPassword { get; set; }

    }
}
