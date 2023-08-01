using DevIO.API.Data;
using DevIO.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevIO.API.Configuration;

public static class IdentityConfig
{
     public static IServiceCollection AddIdentityConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddDefaultTokenProviders();

        return services;
    }
}
