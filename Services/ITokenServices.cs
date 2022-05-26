using System.IdentityModel.Tokens.Jwt;
using bmiWebAPI_3.Models;

namespace bmiWebAPI_3.Services;

public interface ITokenServices
{
    string BuildToken(ApplicationUser appUser);
   
}