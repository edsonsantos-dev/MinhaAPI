using DevIO.API.Data;
using DevIO.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DevIO.API.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddDefaultTokenProviders();

        //JWT

        var appSettingsSection = configuration.GetSection(nameof(AppSettings));
        services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudiences = new List<string>
                {
                    appSettings.ValidoEm
                },
                ValidIssuers = new List<string>
                {
                    appSettings.Emissor
                }
            };
        });

        return services;
    }
}
