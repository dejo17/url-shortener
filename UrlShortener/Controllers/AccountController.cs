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
    [Route("[controller]")] 
    public class AccountController : ControllerBase 
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /**
         * Metoda se poziva kada imamo POST request na URI /account
         * Prima AccountId kao JSON property. Minimalan broj charactera je 5, maksimalan 50.
         * Provjerava u bazi da li postoji taj AccountId.
         *  - Ako da: vraca HTTP status conflict i JSON sa propertyima: success=false i error description
         *  - Ako ne: vraca HTTP status created i JSON sa propertyima: success=true, description message
         *    i random generirani password od 8 znakova
         */
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<AccountResponseBody> GetAccount([FromBody]AccountRequestBody accountModel)
        {
            AccountResponseBody accountResponse = new AccountResponseBody(); //priprema responsea za popuniti
            
            //standardni nacin provjere da li su model binding i model validation prosli ok
            if (ModelState.IsValid) 
            {

                //trazimo da li account postoji da bi znali koji response poslati
                Account account = _accountService.GetAccount(accountModel.AccountId);                   

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
                    Account newAccount = _accountService.CreateAccount(accountModel.AccountId);
                    //popunjavamo response i vracamo status created:
                    accountResponse.success = true;
                    accountResponse.description = "Account created successfully";
                    accountResponse.password = newAccount.Password;
                    return Created("", accountResponse);
                }
            }
            else
            {
                return BadRequest(accountResponse);
            }
        }
    }
}

