using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Models
{
    public class Account
    {
        public string AccountID { get; set; } //ime mora ostati ovako da bi EF prepoznao kao ID (classnameID)
        public string Password { get; set; }
        public ICollection<RegisteredUrl> UrlCollection { get; set; }
    }
}
