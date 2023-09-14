using Fuksi.Loyalty.Module.Auth.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fuksi.Loyalty.Module.Auth.Data;

public class AuthDataContext : IdentityDbContext<AppUser, AppRole, long>
{
    public AuthDataContext(DbContextOptions options) : base(options)
    {
    }
}