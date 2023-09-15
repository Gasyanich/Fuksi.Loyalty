using Fuksi.Loyalty.Module.Auth.Vk.Abstractions;

namespace Fuksi.Loyalty.Module.Auth.Vk;

public class VkUserService : IVkUserService
{
    private readonly IVkApiClientFactory _apiClientFactory;

    public VkUserService(IVkApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task<VkUserInfo> GetUserInfo(string userAccessToken, long vkUserId)
    {
        var apiClient = await _apiClientFactory.Create(userAccessToken);

        var users = await apiClient.Users.GetAsync(new[] {vkUserId});
        var user = users.First();

        return new VkUserInfo(user.FirstName, user.LastName);
    }
}