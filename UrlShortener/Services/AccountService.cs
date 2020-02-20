﻿using System;
using System.Security.Cryptography;
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
                newAccount.Password = Password.Generate(8, 0);    //TODO make random password generator
                _context.Accounts.Add(newAccount);
                int z = _context.SaveChanges();
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
