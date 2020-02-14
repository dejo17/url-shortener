using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //database nece generirati primary key, nego ce se koristiti 
                                                          //onaj koji posalje korisnik prilikom registracije
        public string AccountID { get; set; } //ime mora ostati ovako da bi EF prepoznao kao ID (classnameID)
        public string Password { get; set; }
        public ICollection<RegisteredUrl> UrlCollection { get; set; }
    }
}
