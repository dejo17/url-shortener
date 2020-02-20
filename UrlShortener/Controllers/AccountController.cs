using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")] //placeholder se automatski zamjenjuje sa imenom controllera bez suffixa,
                            //u ovom slucaju: /account. moguce je koristiti i custom path
    public class AccountController : ControllerBase //controlleri nasljeduju od bazne klase ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)   //constructor, DI ubacuje DB context
        {
            _accountService = accountService;
        }

        /**
         * Metoda se poziva kada imamo POST request na path /account
         * Prima AccountId kao JSON property.
         * Provjerava u bazi da li postoji taj AccountId.
         *  -Ako da: vraca HTTP status conflict i JSON sa propertyima: success=false i error description
         *  -Ako ne: vraca HTTP status created i JSON sa propertyima: success=true, description message
         *   i random generirani password od 8 znakova
         */
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<AccountResponseBody> GetAccount([FromBody]AccountRequestBody accountModel)
        {
            //standardni nacin provjere da li su model binding 
            //i model validation prosli ok
            if (ModelState.IsValid) 
            {

                AccountResponseBody accountResponse = new AccountResponseBody(); //priprema responsea za popuniti

                //trazimo da li account postoji da bi znali koji response poslati
                Account account = _accountService.GetAccount(accountModel.AccountId).Result;                   

                if (account != null)
                {
                    //account vec postoji, vracamo 409 conflict
                    accountResponse.success = false;
                    accountResponse.description = "Account exists";
                    return Conflict(accountResponse);
                }
                else
                {
                    //account ne postoji, kreiramo novi
                    Account newAccount = _accountService.CreateAccount(accountModel.AccountId).Result;
                    //popunjavamo response i vracamo status created:
                    accountResponse.success = true;
                    accountResponse.description = "Account created successfully";
                    accountResponse.password = newAccount.Password;
                    return Created("", accountResponse);
                }
            }
            else
            {
                Console.WriteLine("Model is not valid");
                return BadRequest();
            }
        }
    }
}

