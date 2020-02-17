using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{

    public interface IAccountService {
        Task<Account> Authenticate(string username, string password);
        Task<Account> GetAccount(string AccountID);
        Task<Account> CreateAccount(string AccountID);
    }
    public class AccountService : IAccountService
    {
        private readonly UrlShortenerContext _context;

        public AccountService(UrlShortenerContext context) {
            _context = context;
        }


        public async Task<Account> Authenticate(string AccountID, string password)
        {
            //Console.WriteLine("About to authenticate account");
            Account accountToAuthenticate = await _context.Accounts.FindAsync(AccountID);
            if (accountToAuthenticate == null)
            {
                //Console.WriteLine("Account not found");
                return null; //varacamo null ako account ne postoji
            }
            else {
                //ako je user pronaden, provjeravamo password
                //Console.WriteLine("Account found, checking password");
                return (accountToAuthenticate.Password.Equals(password, StringComparison.Ordinal)) ? accountToAuthenticate : null;
            }
        }

        /**
         * 
         */
        public async Task<Account> GetAccount(string AccountID)
        {
            //Console.WriteLine($"Searching for AccountID {AccountID}");
            Account accountFound = await _context.Accounts.FindAsync(AccountID);
            if (accountFound == null)
            {
                //Console.WriteLine("Account not found");
                return null; //varacamo null ako account ne postoji
            }
            else
            {
                //ako je user pronaden, provjeravamo password
                //Console.WriteLine("Account found");
                return accountFound;
            }
        }
        public async Task<Account> CreateAccount(string AccountID) {

            Account account = await GetAccount(AccountID);
            if (account != null)
            {
                //account vec postoji, vracamo ga
                return account;
            }
            else {
                //account ne postoji, kreiramo novi i spremamo u bazu:
                Account newAccount = new Account();
                newAccount.AccountID = AccountID;
                newAccount.Password = "365dfbdr";    //TODO make random password generator
                _context.Accounts.Add(newAccount);
                int z = _context.SaveChanges();
                return newAccount;
            }
        }

    }
}
