using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Domain;
using UrlShortener.Domain.Models;

namespace UrlShortener.Services
{
    public interface IStatisticService
    {
        Dictionary<string, string> GetStatistic(string AccountID);
    }
    public class StatisticService : IStatisticService

    {
        private readonly UrlShortenerContext _context;
        public StatisticService(UrlShortenerContext context)
        {
            _context = context;
        }
        public Dictionary<string, string> GetStatistic(string AccountID)
        {
            var urls = from s in _context.RegisteredUrls
                       select s;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (RegisteredUrl url in urls.ToArray()) {
                if (url.AccountID.Equals(AccountID, StringComparison.Ordinal)) {
                    result.Add(url.LongUrl, url.numberOfCalls.ToString());
                }
            }
            return result;

        }
    }
}
