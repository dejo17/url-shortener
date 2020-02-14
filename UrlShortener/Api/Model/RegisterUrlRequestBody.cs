using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class RegisterUrlRequestBody
    {
        [Required]
        [StringLength(500)]
        [Url]
        public string url { get; set; }
        public int redirectType { get; set;  }
    }
}
