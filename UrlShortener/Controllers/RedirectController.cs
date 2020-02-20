using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("")]
    [ApiController]
    // [Authorize]
    public class RedirectController : ControllerBase
    {
        private readonly IShortenerService _urlService;
        public RedirectController(IShortenerService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet("{ShortUrl}")]
        [AllowAnonymous]
        public ActionResult RedirectToUrl(string ShortUrl)
        {

            var RegisteredUrl = _urlService.GetRegisteredUrl(ShortUrl);
            if (RegisteredUrl != null)
            {
                if (RegisteredUrl.RedirectType > 0)
                {
                    if (RegisteredUrl.RedirectType == 301)
                    {
                        _urlService.IncrementStatistic(RegisteredUrl.RegisteredUrlID);
                        return RedirectPermanent(RegisteredUrl.LongUrl);
                    }
                    else if (RegisteredUrl.RedirectType == 302)
                    {
                        _urlService.IncrementStatistic(RegisteredUrl.RegisteredUrlID);
                        return Redirect(RegisteredUrl.LongUrl);
                    }
                }
                return Redirect(RegisteredUrl.LongUrl); //za svaki slucaj ako je redirectType = null ili 0
            }
            return NotFound();

            /*if (!Response.Headers.ContainsKey("WWW-Authenticate"))
            {
                Response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", "localhost"));
            }
            return Unauthorized();*/

        }   
    }
}