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
        private readonly IUrlService _urlService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(IUrlService urlService, IHttpContextAccessor httpContextAccessor) {
            _urlService = urlService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public ActionResult<RegisterUrlResponseBody> registerUrlAsync([FromBody]RegisterUrlRequestBody request) {

            var account = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
            if (account == null) {
                return Unauthorized();
            }

            RegisteredUrl registeredUrl = _urlService.CreateRegisteredUrl(request, account);
            RegisterUrlResponseBody response = new RegisterUrlResponseBody();
            response.shortUrl = registeredUrl.ShortUrl;            
            return Ok(response);
        
        }
    }
}
