using bmiWebAPI_3.Dtos;
using bmiWebAPI_3.Models;
using bmiWebAPI_3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bmiWebAPI_3.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenServices _tokenService;

    public AuthController(ICosmosDbService cosmosDbService, IPasswordService passwordService,
        ITokenServices tokenService)
    {
        _cosmosDbService = cosmosDbService;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([Bind("Email,Password")] RegisterDto registerDto)
    {
        if (ModelState.IsValid)
            try
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = registerDto.Email,
                    Password = _passwordService.HashPassword(registerDto.Password)
                };
                await _cosmosDbService.AddUserAsync(user);
                var token = _tokenService.BuildToken(user);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        return BadRequest("Invalid user model");
    }


    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<ActionResult<AppUserDto>> SignIn([Bind("Email,Password")] ApplicationUser user)
    {
        if (ModelState.IsValid)
            try
            {
                var email = user.Email;
                var appUser =
                    await _cosmosDbService.GetUsersAsync($"SELECT * FROM Users u WHERE u.email = \"{user.Email}\"");
              
                if (appUser.FirstOrDefault() != null &&
                    _passwordService.VerifyPassword(appUser.FirstOrDefault().Password, user.Password))
                {
                    var token = _tokenService.BuildToken(appUser.FirstOrDefault());
                    var userDto = new AppUserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Token = token
                    };
                    return Ok(userDto);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Ok(e.Message);
            }
        return BadRequest("Invalid user model");
        
        }


         
    /*The HTTP 204 No Content success status response code indicates that a request has succeeded,
     but that the client doesn't need to navigate away from its current page.*/
    [HttpPost("verify")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> verify([Bind("Email,Password")] ApplicationUser user)
    {
        var appUserEmail = User
            .Claims
            .First(c => c.Type == "Email").Value;

        if (appUserEmail == null) return Unauthorized();
        var appUserExists =
            await _cosmosDbService.GetUsersAsync($"SELECT * FROM Users u WHERE u.email = \"{appUserEmail}\"");

        if (appUserExists == null) return Unauthorized();
        return NoContent();
    }


}