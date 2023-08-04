using DevIO.Api.Configuration;
using DevIO.API.Configuration;
using DevIO.API.Extensions;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggin();
builder.AddVersionApi();
builder.ResolveDependencies();
builder.AddIdentityConfiguration();
builder.AddCorsCustom();
builder.AddSwaggerCustom();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

app.UseLoggin();
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

app.UseMiddleware<ExceptionMiddleware>();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwaggerCustom(provider);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
