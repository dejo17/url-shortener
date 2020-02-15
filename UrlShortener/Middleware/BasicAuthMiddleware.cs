using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _realm;

        public BasicAuthMiddleware(RequestDelegate next, string realm)
        {
            _next = next;
            _realm = realm;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"]; //vadimo authorization header
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //iz authorization headera vadimo van username i password (Base64 encoded string, razmaknuti sa :)
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                string username="";
                string password="";
                try
                {
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    username = decodedUsernamePassword.Split(':', 2)[0];
                    password = decodedUsernamePassword.Split(':', 2)[1];
                    Console.WriteLine($"Success: user={username}, pass={password}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Error");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Credentials should be in user:password format and Base64 encoded");
                }
                /*var username = encodedUsernamePassword.Split(':', 2)[0];
                var password = encodedUsernamePassword.Split(':', 2)[1];*/
                // provjera da li je login tocan:
                if (IsAuthorized(username, password))
                {
                    Console.WriteLine("User authorized");
                    await _next.Invoke(context); //ako je, idemo dalje
                    return;
                }
                else {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Wrong credentials");
                }
            }

            //vracamo unauthorized i navodimo authentication type ako header ne postoji
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            // Add realm if it is not null ????
            if (!string.IsNullOrWhiteSpace(_realm))
            {
                context.Response.Headers["WWW-Authenticate"] += $" realm=\"{_realm}\"";
            }
            Console.WriteLine("Missing or wrong format of header");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }



        // Make your own implementation of this
        public bool IsAuthorized(string username, string password)
        {

            return (username.Equals("user", StringComparison.InvariantCultureIgnoreCase)
                 && password.Equals("user", StringComparison.InvariantCultureIgnoreCase));

        }
    }

}
