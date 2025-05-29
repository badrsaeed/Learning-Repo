using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViralWave.Application.Interfaces.Services;

namespace ViralWave.Infrastructure.Services
{
    public static class TokenService
    {
        public static string GetUserId(string authHeader)
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Decode the JWT token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

            return userIdClaim?.Value;
        }
    }
}
