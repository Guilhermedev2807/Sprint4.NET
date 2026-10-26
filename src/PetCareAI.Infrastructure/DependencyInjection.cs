using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Mongo;
using PetCareAI.Infrastructure.Persistence;
using PetCareAI.Infrastructure.Repositories;

namespace PetCareAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AppDbContext>(o => o.UseOracle(cfg.GetConnectionString("Oracle")));
        services.AddSingleton<IMongoClient>(_ => new MongoClient(cfg.GetConnectionString("Mongo")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IVacinaRepository, VacinaRepository>();
        services.AddScoped<IAuditLogRepository, MongoAuditLogRepository>();

        return services;
    }
}