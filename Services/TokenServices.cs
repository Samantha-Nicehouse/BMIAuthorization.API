using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using bmiWebAPI_3.Models;
using Microsoft.IdentityModel.Tokens;

namespace bmiWebAPI_3.Services;

public class TokenServices : ITokenServices
{
    private readonly string securekey = "64A63153-11C1-4919-9133-EFAF99A9B456";

    public string BuildToken(ApplicationUser user)
    {
        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Email", user.Email)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securekey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = "https://localhost:7060",
            Issuer = "https://localhost:7060",
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(claims)
        };
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
    
}