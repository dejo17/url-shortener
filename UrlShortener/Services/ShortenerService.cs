using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{

    public interface IShortenerService
    {
        RegisteredUrl GetRegisteredUrl(string ShortUrl);
        RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody request, string account);
        Dictionary<string, string> GetUrlStatistic(string AccountID);
        void IncrementStatistic(long ID);
    }
    public class ShortenerService : IShortenerService
    {

        private readonly UrlShortenerContext _context;

        public ShortenerService(UrlShortenerContext context)
        {
            _context = context;
        }

        /**
         * metoda vraca registrirani URL
         * 
         * Ako za korisnika 'Account' vec postoji URL kao u 'Request'u, onda vraćamo taj postojeći objekt iz baze
         * Ako URL ne postoji, kreiramo novi RegisteredUrl, spremamo u bazu i vraćamo ga
         */
        public RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody Request, string Account)
        {
            //pokusavamo naci url u bazi podataka. ako postoji, provjeravamo da li pripada ovom accountu.
            //ako je sve zo zadovoljeno, nema potrebe kreirati novi zapis, nego vracamo stari
            RegisteredUrl findUrl = _context.RegisteredUrls.FirstOrDefault(databaseUrl => databaseUrl.LongUrl == Request.url && databaseUrl.AccountID == Account);
            if (findUrl != null)
            {
                return findUrl;
            }
            else
            {
                //URL ne postoji, kreiramo novi zapis u bazi i vracamo novi RegisteredUrl objekt
                RegisteredUrl registeredUrl = new RegisteredUrl() { LongUrl = Request.url, ShortUrl = ShortUrlGenerator.Generate(10), AccountID = Account };
                if (Request.redirectType > 0)
                {
                    registeredUrl.RedirectType = Request.redirectType;
                }
                else
                {
                    registeredUrl.RedirectType = 302;
                }
                _context.RegisteredUrls.Add(registeredUrl);
                _context.SaveChanges();
                return registeredUrl;
            }
        }

        /**
         *  metoda vraca registrirani url objekt iz baze na temelju ShortUrl stringa
         */
        public RegisteredUrl GetRegisteredUrl(string ShortUrl)
        {
            RegisteredUrl findUrl = _context.RegisteredUrls.FirstOrDefault(databaseUrl => databaseUrl.ShortUrl == ShortUrl);
            return findUrl;
        }

        /**
         *  metoda vraca sve registrirane URlove za dani account
         */
        public Dictionary<string, string> GetUrlStatistic(string AccountID)
        {
            var urls = from s in _context.RegisteredUrls
                       select s;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (RegisteredUrl url in urls.ToArray())
            {
                if (url.AccountID.Equals(AccountID, StringComparison.Ordinal))
                {
                    result.Add(url.LongUrl, url.numberOfCalls.ToString());
                }
            }
            return result;

        }

        /**
         *  metoda prima ID registriranog URL objekta i uvecava njegov numberOfCalls za 1, za potrebe statistike
         */
        public void IncrementStatistic(long ID)
        {
            RegisteredUrl findUrl = _context.RegisteredUrls.Find(ID);
            if (findUrl != null)
            {
                findUrl.numberOfCalls += 1;
                _context.Entry(findUrl).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }


        public static class ShortUrlGenerator
        {
            /**
             *  metoda generira short url  string duljine length
             */

            public static string Generate(int length)
            {
                if (length < 1 || length > 128)
                {
                    throw new ArgumentException(nameof(length));
                }
                Random rand = new Random(Environment.TickCount);
                List<char> chars = new List<char>();
                string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

                for (int i = 0; i < length; i++)
                {
                    chars.Add(
                    randomChars.ToCharArray()[rand.Next(0, randomChars.Length)]);
                }

                return new string(chars.ToArray());

            }

        }
    }
}
