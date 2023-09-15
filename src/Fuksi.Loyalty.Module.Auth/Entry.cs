using System.Text;
using Fuksi.Loyalty.Module.Auth.Data;
using Fuksi.Loyalty.Module.Auth.Data.Entities;
using Fuksi.Loyalty.Module.Auth.Options;
using Fuksi.Loyalty.Module.Auth.Vk;
using Fuksi.Loyalty.Module.Auth.Vk.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fuksi.Loyalty.Module.Auth;

public static class Entry
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDataContext>(
            builder => builder.UseNpgsql(configuration.GetConnectionString("AuthDb"))
        );

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("reknbvejntrjkinevjrvke")),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<AuthDataContext>();

        services.AddScoped<IVkApiClientFactory, VkApiClientFactory>();
        services.AddScoped<IVkUserService, VkUserService>();


        services.Configure<FuksiVkAppOptions>(configuration.GetSection("FuksiVkAppOptions"));

        return services;
    }

    public static async Task UseMigrations(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var authDataContext = scope.ServiceProvider.GetRequiredService<AuthDataContext>();
        await authDataContext.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new AppRole
            {
                Name = AppRole.User,
                NormalizedName = AppRole.User
            });

            await roleManager.CreateAsync(new AppRole
            {
                Name = AppRole.Admin,
                NormalizedName = AppRole.Admin
            });
        }
    }
}