namespace Fuksi.Loyalty.Module.Auth.Vk;

public record VkUserInfo(string FirstName, string LastName)
{
    public string FirstName { get; set; } = FirstName;
    public string LastName { get; set; } = LastName;
}