using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{

    public interface IUrlService
    {
        RegisteredUrl GetRegisteredUrl(string Url, string account);
        RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody request, string account);
        Dictionary<string, string> GetUrlStatistic(string AccountID);
        void IncrementStatistic(long ID);
    }
    public class UrlService : IUrlService
    {

        private readonly UrlShortenerContext _context;

        public UrlService(UrlShortenerContext context) {
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
                RegisteredUrl registeredUrl = new RegisteredUrl();
                registeredUrl.LongUrl = request.url;
                registeredUrl.ShortUrl = request.url.Substring(0, 10);
                if (request.redirectType > 0)
                {
                    registeredUrl.RedirectType = request.redirectType;
                }
                else {
                    registeredUrl.RedirectType = 302;
                }
                registeredUrl.AccountID = account;
                _context.RegisteredUrls.Add(registeredUrl);
                _context.SaveChanges();
                return registeredUrl;
            }
        }

        /**
         *  metoda vraca registrirani url objekt
         *  prima puni url i accoundID
         */
        public RegisteredUrl GetRegisteredUrl(string Url, string AccountID)
        {
            RegisteredUrl findUrl = _context.RegisteredUrls.FirstOrDefault(databaseUrl => databaseUrl.ShortUrl == Url && databaseUrl.AccountID == AccountID);
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
         *  metoda uvecava numberofCalls za 1. Prima ID registriranog URL objekta
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
    }
}
