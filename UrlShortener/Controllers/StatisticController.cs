﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Model;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StatisticController(IStatisticService statisticService, IHttpContextAccessor httpContextAccessor) { 
        
            _statisticService = statisticService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("{AccountID}")]
        public ActionResult<Dictionary<string,string>> GetStatistics(string AccountID) {
            if (AccountID.Length >0 && AccountID.Length <= 50)
            {
                var account = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (AccountID.Equals(account, StringComparison.Ordinal)) {
                    return Ok(_statisticService.GetStatistic(AccountID));
                }
                else {
                    return Unauthorized("AccountID's from Authorization header and URI path dont match. You are only allowed to see your own statistic");
                }
            }
            else {
                return BadRequest("AccountID can be 0-50 characters long");
            }

       
        }
    }
}