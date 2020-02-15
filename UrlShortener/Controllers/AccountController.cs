using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")] //placeholder se automatski zamjenjuje sa imenom controllera bez suffixa,
                            //u ovom slucaju: /account. moguce je koristiti i custom path
    public class AccountController : ControllerBase //controlleri nasljeduju od bazne klase ControllerBase
    {
        private readonly UrlShortenerContext _context;

        public AccountController(UrlShortenerContext context)   //constructor, DI ubacuje DB context
        {
            _context = context;
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

                AccountResponseBody accountResponse = new AccountResponseBody();
                Account account = _context.Accounts.FirstOrDefault(account => account.AccountID == accountModel.AccountId);
                
                if (account != null)
                {
                    accountResponse.success = false;
                    accountResponse.description = "Account exists";
                    return Conflict(accountResponse);
                }
                else
                {
                    //account ne postoji, kreiramo novi i spremamo u bazu:
                    Account newAccount = new Account();
                    newAccount.AccountID = accountModel.AccountId;
                    newAccount.Password = "365dfbdr";    //TODO make random password generator
                    _context.Accounts.Add(newAccount);
                    int z = _context.SaveChanges();

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

