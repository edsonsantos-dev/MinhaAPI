using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DevIO.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void ResolveDependencies(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<INotificador, Notificador>();

        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IProdutoService, ProdutoService>();

        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();

        //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //services.AddScoped<IUser, AspNetUser>();

        services.AddScoped<MeuDbContext>();
    }
}
