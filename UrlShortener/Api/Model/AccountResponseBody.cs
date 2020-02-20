using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class AccountResponseBody
    {
#pragma warning disable IDE1006 // Naming Styles
        public bool success { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
        public string description { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
        public string password { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    }
}
