using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetCareAI.Application.Interfaces;
using PetCareAI.Application.Services;
using PetCareAI.Application.Validators;

namespace PetCareAI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IVacinaService, VacinaService>();
        return services;
    }
}