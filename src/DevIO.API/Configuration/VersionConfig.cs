using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Configuration;

public static class VersionConfig
{
    public static void AddVersionApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
