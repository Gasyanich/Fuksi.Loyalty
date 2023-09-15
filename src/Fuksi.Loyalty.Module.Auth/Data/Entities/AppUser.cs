using Microsoft.AspNetCore.Identity;

namespace Fuksi.Loyalty.Module.Auth.Data.Entities;

public class AppUser : IdentityUser<long>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}