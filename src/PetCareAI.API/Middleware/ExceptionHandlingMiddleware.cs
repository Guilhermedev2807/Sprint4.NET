using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetCareAI.Domain.Exceptions;

namespace PetCareAI.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException => (404, "Recurso não encontrado"),
                ValidationException or DomainException => (400, "Requisição inválida"),
                ConflictException => (409, "Conflito"),
                UnauthorizedAppException => (401, "Não autorizado"),
                _ => (500, "Erro interno")
            };

            if (status == 500) log.LogError(ex, "Erro não tratado");
            else log.LogWarning("{Mensagem}", ex.Message);

            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? "Ocorreu um erro inesperado." : ex.Message
            });
        }
    }
}