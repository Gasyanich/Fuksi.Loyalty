using Microsoft.AspNetCore.Identity;

namespace Fuksi.Loyalty.Module.Auth.Data.Entities;

public class AppRole : IdentityRole<long>
{
    public const string User = "USER";
    public const string Admin = "ADMIN";
}