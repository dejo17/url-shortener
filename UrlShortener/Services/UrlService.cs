using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{

    public interface IUrlService
    {
        RegisteredUrl GetRegisteredUrl(string Url, string account);
        RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody request, string account);
    }

    public class UrlService : IUrlService
    {

        private readonly UrlShortenerContext _context;

        public UrlService(UrlShortenerContext context) {
            _context = context;
        }
        public RegisteredUrl CreateRegisteredUrl(RegisterUrlRequestBody request, string account)
        {
            //pokusavamo naci url u bazi podataka. ako postoji, provjeravamo da li pripada ovom accountu.
            //ako je sve zo zadovoljeno, nema potrebe kreirati novi zapis, nego vracamo stari
            //TODO napravi da bude asinhrono
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

        public RegisteredUrl GetRegisteredUrl(string Url, string account)
        {
            RegisteredUrl findUrl = _context.RegisteredUrls.FirstOrDefault(databaseUrl => databaseUrl.ShortUrl == Url && databaseUrl.AccountID == account);
            return findUrl;
        }
    }
}
