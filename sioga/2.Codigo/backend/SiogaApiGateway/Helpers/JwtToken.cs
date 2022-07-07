using SiogaUtils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SiogaApiGateway.Helpers
{
    public class JwtToken
    {
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
