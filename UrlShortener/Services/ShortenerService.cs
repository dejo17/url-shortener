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
         *  kada korisnik pokusa registrirati novi url, provjeravamo da li taj url postoji vec za tog korisnika.
         *  ako da, vracamo vec postojecu skracenu verziju.
         *  ako ne postoji, kreiramo novi registrirani url
         */
        public RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody request, string account)
        {
            //pokusavamo naci url u bazi podataka. ako postoji, provjeravamo da li pripada ovom accountu.
            //ako je sve zo zadovoljeno, nema potrebe kreirati novi zapis, nego vracamo stari
            RegisteredUrl findUrl = _context.RegisteredUrls.FirstOrDefault(databaseUrl => databaseUrl.LongUrl == request.url && databaseUrl.AccountID == account);
            if (findUrl != null)
            {
                return findUrl;
            }
            else
            {
                RegisteredUrl registeredUrl = new RegisteredUrl() { LongUrl = request.url , ShortUrl = ShortUrlGenerator.Generate(10), AccountID = account };
                if (request.redirectType > 0)
                {
                    registeredUrl.RedirectType = request.redirectType;
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
         *  metoda vraca registrirani url objekt
         *  prima puni url i accoundID
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

        /**
         *  metoda generira short url 
         */
        public static class ShortUrlGenerator
        {


            public static string Generate(int length)
            {
                if (length < 1 || length > 128)
                {
                    throw new ArgumentException(nameof(length));
                }
                Random rand = new Random(Environment.TickCount);
                List<char> chars = new List<char>();
                string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

                for (int i = 0; i < length; i++) {
                    chars.Add(
                    randomChars.ToCharArray()[rand.Next(0, randomChars.Length)]);
                }

                return new string(chars.ToArray());

            }

        }
    }
}
