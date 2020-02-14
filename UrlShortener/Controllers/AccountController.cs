using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Domain;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")] //placeholder se automatski zamjenjuje sa imenom controllera bez suffixa, u ovom slucaju: /account
    public class AccountController : ControllerBase //controlleri nasljeduju od bazne klase ControllerBase
    {
        private readonly UrlShortenerContext _context;

        public AccountController(UrlShortenerContext context) {
            _context = context;
        }

        private static List<AccountRequestBody> accounts = new List<AccountRequestBody>();
        [HttpPost]
        public ActionResult<AccountResponseBody> GetAccount([FromBody]AccountRequestBody accountModel)
        {
            Console.WriteLine("Post request:");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model is not valid");
                return BadRequest();
                
            }
            else
            {
                AccountResponseBody accountResponse = new AccountResponseBody();
                
                if (accounts.Contains(accountModel))
                {
                    accountResponse.success = false;
                    accountResponse.description = "Account exists";
                    Console.WriteLine(accountModel.AccountId);
                    Console.WriteLine($"There are {accounts.Count} elements in the list");
                    return Conflict(accountResponse);
                }
                else
                {
                    accounts.Add(accountModel);
                    accountResponse.success = true;
                    accountResponse.description = "Account created successfully";

                    accountResponse.password = "365dfbdr"; //TODO make random password generator
                    Console.WriteLine(accountModel.AccountId);
                    Console.WriteLine($"There are {accounts.Count} elements in the list");
                    return Created("", accountResponse);
                }

            }
        }
    }
}
