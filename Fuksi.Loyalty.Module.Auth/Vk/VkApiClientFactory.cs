using Fuksi.Loyalty.Module.Auth.Vk.Abstractions;
using VkNet;
using VkNet.Model;

namespace Fuksi.Loyalty.Module.Auth.Vk;

public class VkApiClientFactory : IVkApiClientFactory
{
    public async Task<VkApi> Create(string userAccessToken)
    {
        var vkApi = new VkApi();
        await vkApi.AuthorizeAsync(new ApiAuthParams
        {
            AccessToken = userAccessToken
        });

        return vkApi;
    }
}