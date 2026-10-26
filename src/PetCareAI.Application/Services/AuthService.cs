using FluentValidation;
using PetCareAI.Application.Common;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Services;

public class AuthService(
    IUsuarioRepository usuarios,
    IPasswordHasher hasher,
    ITokenService tokens,
    IAuditLogRepository audit,
    IValidator<RegisterDto> registerValidator,
    IValidator<LoginDto> loginValidator) : IAuthService
{
    public async Task<AuthResponseDto> RegistrarAsync(RegisterDto dto)
    {
        await registerValidator.ValidateAndThrowAsync(dto);

        if (await usuarios.GetByEmailAsync(dto.Email) is not null)
            throw new ConflictException("E-mail já cadastrado.");

        var usuario = new Usuario(dto.Nome, dto.Email, hasher.Hash(dto.Senha));
        await usuarios.AddAsync(usuario);

        await audit.RegistrarAsync(new AuditLog
        {
            UsuarioId = usuario.Id,
            Acao = AuditActions.UsuarioRegistrado,
            Entidade = nameof(Usuario),
            Detalhes = usuario.Id.ToString()
        });

        return new AuthResponseDto(tokens.Gerar(usuario), usuario.Nome, usuario.Email);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        await loginValidator.ValidateAndThrowAsync(dto);

        var usuario = await usuarios.GetByEmailAsync(dto.Email);
        if (usuario is null || !hasher.Verificar(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        await audit.RegistrarAsync(new AuditLog
        {
            UsuarioId = usuario.Id,
            Acao = AuditActions.Login,
            Entidade = nameof(Usuario),
            Detalhes = usuario.Id.ToString()
        });

        return new AuthResponseDto(tokens.Gerar(usuario), usuario.Nome, usuario.Email);
    }
}