using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Domain.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //database nece generirati primary key, nego ce se koristiti 
                                                          //onaj koji posalje korisnik prilikom registracije
        [Column(TypeName = "varchar(50)")]
        public string AccountID { get; set; } //ime mora ostati ovako da bi EF prepoznao kao ID (classnameID)
        [Column(TypeName = "varchar(15)")]
        public string Password { get; set; }
        public ICollection<RegisteredUrl> UrlCollection { get; set; } //navigation property
    }
}
