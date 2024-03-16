using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using dev_backend_net.Models;
namespace dev_backend_net.Middleware
{
    public class TokenVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        public TokenVerificationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Authorization"];
            string[] tokenParts = token.Split(' ');
            string jwToken = tokenParts.Length > 1 ? tokenParts[1] : null;
            if (!IsValidToken(jwToken))
            {
                var json = JsonConvert.SerializeObject(new
                {
                    error = "Token inválido",
                    message = "El token proporcionado no es válido o ha expirado.",
                    confirm = false
                });
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(json);
                return;
            }
            await _next(context);
        }
        private bool IsValidToken(string token)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            var key = Encoding.UTF8.GetBytes(jwt.Key);
            // var key = new SymmetricSecurityKey(keyBytes);
            Console.WriteLine(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes("passwordsecret1234");
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
                var expirationDate = tokenHandler.ReadToken(token).ValidTo;
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                if (expirationDate < DateTime.UtcNow)
                {
                    Console.WriteLine("El token ha expirado.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar el token: " + ex.Message);
                return false;
            }

        }
    }

    public static class TokenVerificationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenVerificationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenVerificationMiddleware>();
        }
    }
}