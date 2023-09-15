namespace Fuksi.Loyalty.Module.Auth.Options;

public class FuksiVkAppOptions
{
    public ulong Id { get; set; }

    public string Secret { get; set; }

    public string RedirectUri { get; set; }

    public string Scope { get; set; }

    public string ResponseType { get; set; }

    public string AuthorizeUri { get; set; }

    public string TokenUri { get; set; }
}