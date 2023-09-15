using VkNet;

namespace Fuksi.Loyalty.Module.Auth.Vk.Abstractions;

public interface IVkApiClientFactory
{
    Task<VkApi> Create(string userAccessToken);
}