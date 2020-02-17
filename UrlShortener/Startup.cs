using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;
using Microsoft.AspNetCore.Authentication;
using UrlShortener.Services;
using UrlShortener.Authentication;
using Microsoft.AspNetCore.Http;

namespace UrlShortener
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UrlShortenerContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("UrlShortenerContext")));
            //options.UseInMemoryDatabase("UrlShortenerContext"));

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //sluzi da mozemo pristupiti trenutnom useru
            // configure DI for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUrlService, UrlService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMiddleware<BasicAuthMiddleware>("test"); //TODO refactor string
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
