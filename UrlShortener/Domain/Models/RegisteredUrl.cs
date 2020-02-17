using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Models
{
    public class RegisteredUrl
    {
        public long RegisteredUrlID { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string LongUrl { get; set;  }
        [Column(TypeName = "varchar(50)")]
        public string ShortUrl { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string AccountID { get; set; }
        public Account account { get; set;  } //navigation property
        public int RedirectType { get; set; }
        public int numberOfCalls { get; set; }

    }
}
