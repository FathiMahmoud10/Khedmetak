using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Khedmetak.DAL.Entities;      // ✅ أضف ده
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Khedmetak.BLL.Services.Implementation
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user, IList<string> roles)  // ✅ User مش IdentityUser
        {
            var jwt = _config.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ✅ ToString() لأن Id = int
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name),                    // ✅ أضفنا الاسم
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(int.Parse(jwt["ExpiryInDays"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}