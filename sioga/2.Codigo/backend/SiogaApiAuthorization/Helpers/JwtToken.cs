using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SiogaUtils;

namespace SiogaApiAuthorization.Helpers
{
    public class JwtToken
    {

        public static async Task<string> CreateJWTAsync(string userData, List<string> roles, string issuer, string audience, string symSec, int minutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = await CreateClaimsIdentities(userData, roles);

            // Create JWToken
            var token = tokenHandler.CreateJwtSecurityToken(issuer: issuer,
                audience: audience,
                subject: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials:
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.Default.GetBytes(symSec)),
                        SecurityAlgorithms.HmacSha256Signature));

            return tokenHandler.WriteToken(token);
        }

        public static Task<ClaimsIdentity> CreateClaimsIdentities(string userData, List<string> roles)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("userdata", userData));

            foreach (var userRole in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole));
            }


            return Task.FromResult(claimsIdentity);
        }

        public static string GetClaim(string authHeader, string claimName)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = authHeader.Replace("Bearer ", "");
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                return claimValue;
            }
            catch (Exception)
            {
                //TODO: Logger.Error
                return null;
            }
        }

        public static JwtUserData GetUserData(string authHeader, string key)
        {
            var claimsUsertData = GetClaim(authHeader, "userdata");

            return JsonOptions<JwtUserData>.ToDecript(claimsUsertData, key);
        }
    }

    public class JwtUserData
    {
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public JwtUserData()
        {
            Roles = new List<string>();
        }
    }
}
