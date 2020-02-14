using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Models
{
    public class RegisteredUrl
    {
        public long RegisteredUrlID { get; set; }
        public string LongUrl { get; set;  }
        public string ShortUrl { get; set; }
        public string AccountID { get; set; }
        public Account account { get; set;  } //navigation property
        public int RedirectType { get; set; }

    }
}
