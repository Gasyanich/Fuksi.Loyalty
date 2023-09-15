using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Fuksi.Loyalty.Module.Auth.Data;
using Fuksi.Loyalty.Module.Auth.Data.Entities;
using Fuksi.Loyalty.Module.Auth.Options;
using Fuksi.Loyalty.Module.Auth.Vk;
using Fuksi.Loyalty.Module.Auth.Vk.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Fuksi.Loyalty.Module.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FuksiVkAppOptions _options;
    private readonly HttpClient _httpClient;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDataContext _dataContext;
    private readonly IVkUserService _vkUserService;

    public AuthController(IOptions<FuksiVkAppOptions> options,
        HttpClient httpClient,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        AuthDataContext dataContext,
        IVkUserService vkUserService)
    {
        _httpClient = httpClient;
        _signInManager = signInManager;
        _userManager = userManager;
        _dataContext = dataContext;
        _vkUserService = vkUserService;
        _options = options.Value;
    }

    [HttpGet("vk")]
    public IActionResult SignInRedirect([FromQuery] string returnUrl)
    {
        var queryParams = new Dictionary<string, string>
        {
            {"client_id", _options.Id.ToString()},
            {"display", "page"},
            {"redirect_uri", _options.RedirectUri},
            {"scope", _options.Scope},
            {"response_type", _options.ResponseType},
            {"state", returnUrl},
        };

        var vkAuthUri = QueryHelpers.AddQueryString(_options.AuthorizeUri, queryParams!);

        return Ok(new {redirectUri = vkAuthUri});
    }

    [HttpGet("vk-callback")]
    public async Task<IActionResult> SignInCallback(
        [FromQuery] string code,
        [FromQuery(Name = "state")] string returnUrl)
    {
        var queryParams = new Dictionary<string, string>
        {
            {"client_id", _options.Id.ToString()},
            {"client_secret", _options.Secret},
            {"redirect_uri", _options.RedirectUri},
            {"code", code}
        };

        var accessTokenUri = QueryHelpers.AddQueryString(_options.TokenUri, queryParams!);

        var vkAccessTokenResponse = await _httpClient.GetAsync(accessTokenUri);

        var response = await vkAccessTokenResponse.Content.ReadFromJsonAsync<VkAccessTokenResponse>();

        var user = await _userManager.FindByLoginAsync(VkConstants.VkProviderName, response!.UserId.ToString());
        if (user is null)
            user = await CreateUser(response);

        await _signInManager.SignInAsync(user, true);

        return Redirect(returnUrl);
    }

    private async Task<AppUser> CreateUser(VkAccessTokenResponse vkAccessTokenResponse)
    {
        var (accessToken, _, userId, email) = vkAccessTokenResponse;

        var vkUserInfo = await _vkUserService.GetUserInfo(accessToken, userId);

        var user = new AppUser
        {
            FirstName = vkUserInfo.FirstName,
            LastName = vkUserInfo.LastName,
            Email = email,
            UserName = Guid.NewGuid().ToString("N")
        };

        await _userManager.CreateAsync(user);
        await _userManager.AddToRoleAsync(user, AppRole.User);

        await _userManager.AddLoginAsync(
            user,
            new UserLoginInfo(VkConstants.VkProviderName, userId.ToString(), VkConstants.VkProviderDisplayName)
        );

        _dataContext.UserTokens.Add(new IdentityUserToken<long>
        {
            LoginProvider = VkConstants.VkProviderName,
            Name = VkConstants.VkAccessToken,
            UserId = user.Id,
            Value = accessToken
        });

        await _dataContext.SaveChangesAsync();

        return user;
    }
}