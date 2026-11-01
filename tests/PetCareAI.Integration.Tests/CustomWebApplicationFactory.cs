using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Persistence;

namespace PetCareAI.Integration.Tests;

public class FakeAuditLogRepository : IAuditLogRepository
{
    public Task RegistrarAsync(AuditLog log) => Task.CompletedTask;
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-de-teste-com-mais-de-32-caracteres!!",
                ["Jwt:Issuer"] = "PetCareAI",
                ["Jwt:Audience"] = "PetCareAI",
                ["Jwt:ExpiresMinutes"] = "60",
                ["ConnectionStrings:Oracle"] = "User Id=x;Password=x;Data Source=localhost:1521/x",
                ["ConnectionStrings:Mongo"] = "mongodb://localhost:27017"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descritores = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                         || d.ServiceType == typeof(DbContextOptions)
                         || (d.ServiceType.IsGenericType
                             && d.ServiceType.GetGenericTypeDefinition().Name.StartsWith("IDbContextOptionsConfiguration")))
                .ToList();
            foreach (var d in descritores) services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_dbName));

            services.RemoveAll<IAuditLogRepository>();
            services.AddScoped<IAuditLogRepository, FakeAuditLogRepository>();
        });
    }
}