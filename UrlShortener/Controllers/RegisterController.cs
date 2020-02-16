using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Api.Model;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        private readonly UrlShortenerContext _context;
        public RegisterController(UrlShortenerContext context) {
            _context = context;
        }
        [HttpPost]
        public ActionResult<RegisterUrlResponseBody> registerUrl([FromBody]RegisterUrlRequestBody request) {

            RegisterUrlResponseBody response = new RegisterUrlResponseBody();
            if (request.url == null)
            {
                return BadRequest();
            }
            else {
                RegisteredUrl registeredUrl = new RegisteredUrl();
                registeredUrl.LongUrl = request.url;
                registeredUrl.ShortUrl = request.url.Substring(0, 10);
                registeredUrl.RedirectType = request.redirectType;
                response.shortUrl = registeredUrl.ShortUrl;
                _context.RegisteredUrls.Add(registeredUrl);
                _context.SaveChanges();

            }
            return Ok(response);
        
        }
    }
}
