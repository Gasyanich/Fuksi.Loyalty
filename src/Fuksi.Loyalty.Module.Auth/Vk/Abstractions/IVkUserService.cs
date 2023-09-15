namespace Fuksi.Loyalty.Module.Auth.Vk.Abstractions;

public interface IVkUserService
{
    Task<VkUserInfo> GetUserInfo(string userAccessToken, long vkUserId);
}