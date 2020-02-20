using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Api.Model;
using UrlShortener.Domain.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        private readonly IShortenerService _urlService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(IShortenerService urlService, IHttpContextAccessor httpContextAccessor) {
            _urlService = urlService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public ActionResult<RegisterUrlResponseBody> RegisterUrl([FromBody]RegisterUrlRequestBody request) {

            var account = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (account == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
      
            RegisteredUrl registeredUrl = _urlService.CreateRegisteredUrl(request, account);
            RegisterUrlResponseBody response = new RegisterUrlResponseBody() { shortUrl = registeredUrl.ShortUrl};       
            return Ok(response);
        
        }
    }
}
