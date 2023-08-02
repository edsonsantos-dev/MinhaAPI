namespace DevIO.API.Configuration;

public static class CorsCustomConfig
{
    public static void AddCorsCustom(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddCors(options =>
        {
            options.AddPolicy("Development",
                x => x.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            options.AddPolicy("Production",
                x => x.WithMethods("GET")
                    .WithOrigins("http://escode.net")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader());
        });
    }
}
