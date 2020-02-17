using System;
using System.Collections.Generic;
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult help() {

            return Ok();
        }




    }
}