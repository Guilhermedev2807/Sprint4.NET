using FluentAssertions;
using FluentValidation;
using Moq;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Services;
using PetCareAI.Application.Validators;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Tests;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _repo = new();
    private readonly Mock<IAuditLogRepository> _audit = new();
    private readonly PetService _service;

    public PetServiceTests()
    {
        _service = new PetService(_repo.Object, _audit.Object, new PetRequestValidator());
    }

    private static PetRequestDto DtoValido() =>
        new("Rex", "Cachorro", "Labrador", DateTime.UtcNow.AddYears(-2), 12.5m);

    [Fact]
    public async Task Criar_Com_Dados_Validos_Deve_Salvar_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _service.CriarAsync(userId, DtoValido());

        // Assert
        result.Nome.Should().Be("Rex");
        _repo.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public async Task Criar_Com_Peso_Invalido_Deve_Lancar_ValidationException()
    {
        // Arrange
        var dto = DtoValido() with { Peso = 0 };

        // Act
        var act = () => _service.CriarAsync(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task Obter_Pet_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Pet?)null);

        // Act
        var act = () => _service.ObterAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Obter_Pet_Existente_Deve_Retornar_Dto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pet = new Pet(userId, "Thor", "Gato", null, DateTime.UtcNow.AddYears(-1), 4m);
        _repo.Setup(r => r.GetByIdAsync(pet.Id, userId)).ReturnsAsync(pet);

        // Act
        var result = await _service.ObterAsync(pet.Id, userId);

        // Assert
        result.Nome.Should().Be("Thor");
    }

    [Fact]
    public async Task Remover_Pet_Existente_Deve_Excluir_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pet = new Pet(userId, "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-1), 8m);
        _repo.Setup(r => r.GetByIdAsync(pet.Id, userId)).ReturnsAsync(pet);

        // Act
        await _service.RemoverAsync(pet.Id, userId);

        // Assert
        _repo.Verify(r => r.DeleteAsync(pet), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public async Task Listar_Deve_Retornar_Resultado_Paginado()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pets = new[] { new Pet(userId, "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-1), 8m) };
        _repo.Setup(r => r.ListAsync(userId, null, null, 1, 10)).ReturnsAsync((pets, 1));

        // Act
        var result = await _service.ListarAsync(userId, new PetQueryParams());

        // Assert
        result.Total.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}