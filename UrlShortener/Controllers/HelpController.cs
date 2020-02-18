using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        public ActionResult getHelp() {
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot" , "help.html");

            return PhysicalFile(file, "text/html ");
        }
    }
    }
}