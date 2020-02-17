using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RedirectController(IUrlService urlService, IHttpContextAccessor httpContextAccessor)
        {
            _urlService = urlService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{ShortUrl}")]
        public ActionResult RedirectToUrl(string ShortUrl)
        {
            var account = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (account != null)
            {
                var RegisteredUrl = _urlService.GetRegisteredUrl(ShortUrl, account);
                if (RegisteredUrl != null)
                {
                    if (RegisteredUrl.RedirectType > 0)
                    {
                        if (RegisteredUrl.RedirectType == 301)
                        {
                            return RedirectPermanent(RegisteredUrl.LongUrl);
                        }
                        else if (RegisteredUrl.RedirectType == 302)
                        {
                            return Redirect(RegisteredUrl.LongUrl);
                        }
                    }
                    return Redirect(RegisteredUrl.LongUrl); //za svaki slucaj ako je redirectType = null ili 0
                }
                return NotFound();
            }
            if (!Response.Headers.ContainsKey("WWW-Authenticate"))
            {
                Response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", "localhost"));
            }
            return Unauthorized();

        }

    }
}