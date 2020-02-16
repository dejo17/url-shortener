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
        Task<IEnumerable<Account>> GetAll(); //TODO napravit reviziju ovoga kasnije
    }
    public class AccountService : IAccountService
    {
        private readonly UrlShortenerContext _context;

        public AccountService(UrlShortenerContext context) {
            _context = context;
        }


        public async Task<Account> Authenticate(string username, string password)
        {
            Console.WriteLine("About to authenticate user");
            Account accountToAuthenticate = await _context.Accounts.FindAsync(username);
            if (accountToAuthenticate == null)
            {
                Console.WriteLine("User not found");
                return null; //varacamo null ako account ne postoji
            }
            else {
                //ako je user pronaden, provjeravamo password
                Console.WriteLine("User found, checking password");
                return (accountToAuthenticate.Password.Equals(password, StringComparison.Ordinal)) ? accountToAuthenticate : null;
            }
        }

        public Task<IEnumerable<Account>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
