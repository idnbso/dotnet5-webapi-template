using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Application.Infrastructure.Auth
{
    public class JWTAuthenticationManager : IJWTAuthenticationManager
    {
        IDictionary<string, string> users = new Dictionary<string, string>
        {
            { "test1", "test1" },
            { "test2", "password2" }
        };

        private readonly IHttpContextAccessor _context;
        private readonly string _tokenKey;

        public JWTAuthenticationManager(string tokenKey, IHttpContextAccessor context)
        {
            _tokenKey = tokenKey;
            _context = context;
        }

        public string Authenticate(string username = null, string password = null)
        {
            var identity = _context.HttpContext.User.Identity;

            // Validate User
            if (identity is null || string.IsNullOrWhiteSpace(identity.Name))
            {
                return null;
            }

            // Build Token
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identity.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Create Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}