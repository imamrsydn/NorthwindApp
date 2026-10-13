using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Noerthwind.API
{
    public static class JwtToken
    {
        //public const string secretKey = "Northwind-API-Training-Super-Secret-Key-2026!";

        public static SymmetricSecurityKey GenerateSigningKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private static JwtHeader GetHeader(SymmetricSecurityKey signingKey)
        {
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            return new JwtHeader(credentials);
        }

        private static long GetExpireDuration()
        {
            var expireDate = DateTimeOffset.UtcNow.AddHours(1);
            return expireDate.ToUnixTimeSeconds();
        }

        private static JwtPayload GetPayload(string subject, string user, string[] roles, string audience, string issuer)
        {
            var payload = new JwtPayload
            {
                { "sub", subject },
                { "name", user },
                { "roles", roles },
                { "aud", audience }, // Diperbaiki dari "audiencw"
                { "iss", issuer },
                { "exp", GetExpireDuration() }
            };
            return payload;
        }

        public static string GetStringToken(
            string subject,
            string user,
            string[] roles,
            string secretKey,
            string audience = "NorthwindClient",
            string issuer = "http://localhost:5000/")
        {
            var signingKey = GenerateSigningKey(secretKey);
            var header = GetHeader(signingKey);
            var payload = GetPayload(subject, user, roles, audience, issuer);

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}