using Elmah.Io.AspNetCore;

namespace DevIO.API.Configuration;

public static class LoggerConfig
{
    public static void AddLoggin(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<ElmahIoOptions>(builder.Configuration.GetSection("ElmahIo"));
        services.AddElmahIo();
    }

    public static void UseLoggin(this WebApplication app)
    {
        app.UseElmahIo();
    }
}
