using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fuksi.Loyalty.Module.Auth.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Fuksi.Loyalty.Module.Auth.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public TokenController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Token()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("reknbvejntrjkinevjrvke")),
            SecurityAlgorithms.HmacSha512
        );
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(30),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encodedJwt = tokenHandler.WriteToken(token);

        return Ok(new {accessToken = encodedJwt});
    }
}