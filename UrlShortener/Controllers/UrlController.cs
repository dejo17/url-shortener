using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UrlShortener.Api.Model;
using UrlShortener.Domain.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize]
    public class UrlController : ControllerBase
    {
        private readonly IShortenerService _urlService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlController(IShortenerService urlService, IHttpContextAccessor httpContextAccessor) {
            _urlService = urlService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("/register")]
        public ActionResult<RegisterUrlResponseBody> RegisterUrl([FromBody]RegisterUrlRequestBody request) {


            var accountClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountClaim == null) {
                return Unauthorized();
            }
            var account = accountClaim.Value;
            if (account == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
      
            RegisteredUrl registeredUrl = _urlService.CreateRegisteredUrl(request, account);
            RegisterUrlResponseBody response = new RegisterUrlResponseBody() { shortUrl = $"http://localhost:6500/{registeredUrl.ShortUrl}"};       
            return Created("", response);
        
        }

        /**
         *  metoda vraća sve URLove korisnika AccounID, te koliko je puta svaki od njih pozvan
         *  AcountID path varijabla treba biti ista kao username iz Authorization headera,
         *  inače vraćamo Unauthorized
         */
        [HttpGet("/statistic/{AccountID}")]
        public ActionResult<Dictionary<string, string>> GetStatistics(string AccountID)
        {
            if (AccountID.Length > 0 && AccountID.Length <= 50)
            {
                var account = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (AccountID.Equals(account, StringComparison.Ordinal))
                {
                    return Ok(_urlService.GetUrlStatistic(AccountID));
                }
                else
                {
                    return Unauthorized("AccountID's from Authorization header and URI path dont match. You are only allowed to see your own statistic");
                }
            }
            else
            {
                return BadRequest("AccountID can be 0-50 characters long");
            }


        }
    }
}
