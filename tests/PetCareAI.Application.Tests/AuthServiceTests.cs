using FluentAssertions;
using FluentValidation;
using Moq;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;
using PetCareAI.Application.Services;
using PetCareAI.Application.Validators;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IAuditLogRepository> _audit = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hash");
        _tokens.Setup(t => t.Gerar(It.IsAny<Usuario>())).Returns("token-fake");
        _service = new AuthService(_usuarios.Object, _hasher.Object, _tokens.Object, _audit.Object,
            new RegisterValidator(), new LoginValidator());
    }

    [Fact]
    public async Task Registrar_Com_Email_Novo_Deve_Retornar_Token()
    {
        // Arrange
        _usuarios.Setup(u => u.GetByEmailAsync("ana@teste.com")).ReturnsAsync((Usuario?)null);

        // Act
        var result = await _service.RegistrarAsync(new RegisterDto("Ana", "ana@teste.com", "123456"));

        // Assert
        result.Token.Should().Be("token-fake");
        _usuarios.Verify(u => u.AddAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task Registrar_Com_Email_Existente_Deve_Lancar_Conflict()
    {
        // Arrange
        var existente = new Usuario("Ana", "ana@teste.com", "hash");
        _usuarios.Setup(u => u.GetByEmailAsync("ana@teste.com")).ReturnsAsync(existente);

        // Act
        var act = () => _service.RegistrarAsync(new RegisterDto("Ana", "ana@teste.com", "123456"));

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Login_Com_Senha_Incorreta_Deve_Lancar_Unauthorized()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@teste.com", "hash");
        _usuarios.Setup(u => u.GetByEmailAsync("ana@teste.com")).ReturnsAsync(usuario);
        _hasher.Setup(h => h.Verificar("errada", "hash")).Returns(false);

        // Act
        var act = () => _service.LoginAsync(new LoginDto("ana@teste.com", "errada"));

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }

    [Fact]
    public async Task Login_Com_Credenciais_Corretas_Deve_Retornar_Token()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@teste.com", "hash");
        _usuarios.Setup(u => u.GetByEmailAsync("ana@teste.com")).ReturnsAsync(usuario);
        _hasher.Setup(h => h.Verificar("123456", "hash")).Returns(true);

        // Act
        var result = await _service.LoginAsync(new LoginDto("ana@teste.com", "123456"));

        // Assert
        result.Token.Should().Be("token-fake");
    }
}