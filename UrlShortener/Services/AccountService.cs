using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{

    public interface IAccountService {
       Account Authenticate(string username, string password);
        Account GetAccount(string AccountID);
       Account CreateAccount(string AccountID);
    }
    public class AccountService : IAccountService
    {
        private readonly UrlShortenerContext _context;

        public AccountService(UrlShortenerContext context) {
            _context = context;
        }

        /**
         *  metoda autentificira korisnika
         *  vraca null ako korisnik ne postoji
         *  vraca Account objekt ako je autentifikaicja prosla OK
         */
        public Account Authenticate(string AccountID, string password)
        {
            Account accountToAuthenticate = _context.Accounts.Find(AccountID);
            if (accountToAuthenticate == null)
            {
                return null; //varacamo null ako account ne postoji
            }
            else {
                //ako je user pronaden, provjeravamo password i vracamo Account objekt
                return (accountToAuthenticate.Password.Equals(password, StringComparison.Ordinal)) ? accountToAuthenticate : null;
            }
        }

        /**
         * 
         */
        public Account GetAccount(string AccountID)
        {
            Account accountFound = _context.Accounts.Find(AccountID);
            if (accountFound == null)
            {
                return null; //varacamo null ako account ne postoji
            }
            else
            {
                //user pronaden, vracamo Account objekt
                return accountFound;
            }
        }
        public Account CreateAccount(string AccountID) {

            Account account = GetAccount(AccountID);
            if (account != null)
            {
                //account vec postoji, vracamo ga
                return account;
            }
            else {
                //account ne postoji, kreiramo novi i spremamo u bazu:
                Account newAccount = new Account()
                {
                    AccountID = AccountID,
                    Password = Password.Generate(8, 0)
                };

                _context.Accounts.Add(newAccount);
                _context.SaveChanges();
                return newAccount;
            }
        }

        /**
        *  metoda generira password
        *  ovo je copy-paste source koda kao u Membership.GeneratePassword iz .NET Frameworka
        *  https://github.com/Microsoft/referencesource/blob/master/System.Web/Security/Membership.cs
        */
        public static class Password
        {
            private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

            public static string Generate(int length, int numberOfNonAlphanumericCharacters)
            {
                if (length < 1 || length > 128)
                {
                    throw new ArgumentException(nameof(length));
                }

                if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
                {
                    throw new ArgumentException(nameof(numberOfNonAlphanumericCharacters));
                }

                using (var rng = RandomNumberGenerator.Create())
                {
                    var byteBuffer = new byte[length];

                    rng.GetBytes(byteBuffer);

                    var count = 0;
                    var characterBuffer = new char[length];

                    for (var iter = 0; iter < length; iter++)
                    {
                        var i = byteBuffer[iter] % 87;

                        if (i < 10)
                        {
                            characterBuffer[iter] = (char)('0' + i);
                        }
                        else if (i < 36)
                        {
                            characterBuffer[iter] = (char)('A' + i - 10);
                        }
                        else if (i < 62)
                        {
                            characterBuffer[iter] = (char)('a' + i - 36);
                           
                        }
                       else
                        {
                            characterBuffer[iter] = Punctuations[i - 62];
                            count++;
                        }
                    }

                    if (count >= numberOfNonAlphanumericCharacters)
                    {
                        return new string(characterBuffer);
                    }

                    int j;
                    var rand = new Random();

                    for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
                    {
                        int k;
                        do
                        {
                            k = rand.Next(0, length);
                        }
                        while (!char.IsLetterOrDigit(characterBuffer[k]));

                        characterBuffer[k] = Punctuations[rand.Next(0, Punctuations.Length)];
                    }

                    return new string(characterBuffer);
                }
            }

        }
    }
}
