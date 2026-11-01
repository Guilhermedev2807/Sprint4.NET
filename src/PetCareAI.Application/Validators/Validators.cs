using FluentValidation;
using PetCareAI.Application.DTOs;

namespace PetCareAI.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(160);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Senha).NotEmpty();
    }
}

public class PetRequestValidator : AbstractValidator<PetRequestDto>
{
    public PetRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Especie).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Raca).MaximumLength(80);
        RuleFor(x => x.Peso).GreaterThan(0).LessThan(1000);
        RuleFor(x => x.DataNascimento).LessThanOrEqualTo(_ => DateTime.UtcNow)
            .WithMessage("Data de nascimento não pode ser futura.");
    }
}
public class ConsultaRequestValidator : AbstractValidator<ConsultaRequestDto>
{
    public ConsultaRequestValidator()
    {
        RuleFor(x => x.PetId).NotEmpty();
        RuleFor(x => x.Veterinario).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(300);
    }
}

public class VacinaRequestValidator : AbstractValidator<VacinaRequestDto>
{
    public VacinaRequestValidator()
    {
        RuleFor(x => x.PetId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
    }
}