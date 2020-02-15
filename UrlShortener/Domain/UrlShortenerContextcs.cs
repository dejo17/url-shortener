using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Models;

namespace UrlShortener.Domain
{
    public class UrlShortenerContext : DbContext
    {
        public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<RegisteredUrl> RegisteredUrls { get; set; }
    }
}
