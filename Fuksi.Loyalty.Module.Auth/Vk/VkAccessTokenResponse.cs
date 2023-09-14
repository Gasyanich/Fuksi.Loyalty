using System.Text.Json.Serialization;

namespace Fuksi.Loyalty.Module.Auth.Vk;

public record VkAccessTokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = null!;

    [JsonPropertyName("expires_in")] public long ExpiresIn { get; set; }

    [JsonPropertyName("user_id")] public long UserId { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; } = null!;

    public void Deconstruct(out string accessToken, out long expiresIn, out long userId, out string email)
    {
        accessToken = AccessToken;
        expiresIn = ExpiresIn;
        userId = UserId;
        email = Email;
    }
}