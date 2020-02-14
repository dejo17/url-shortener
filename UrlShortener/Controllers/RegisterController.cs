using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Api.Model;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public ActionResult<RegisterUrlResponseBody> registerUrl([FromBody]RegisterUrlRequestBody request) {

            RegisterUrlResponseBody response = new RegisterUrlResponseBody();
            if (request.url == null)
            {
                return BadRequest();
            }
            else {
                response.shortUrl = request.url.Substring(0, 10);
            }
            return Ok(response);
        
        }
    }
}
