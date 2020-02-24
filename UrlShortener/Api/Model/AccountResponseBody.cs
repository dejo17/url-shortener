using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class AccountResponseBody
    {
        public bool success { get; set; }
        public string description { get; set; }
        public string password { get; set; }
    }
}
