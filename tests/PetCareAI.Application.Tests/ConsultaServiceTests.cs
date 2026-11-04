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

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _repo = new();
    private readonly Mock<IPetRepository> _pets = new();
    private readonly Mock<IAuditLogRepository> _audit = new();
    private readonly ConsultaService _service;

    public ConsultaServiceTests()
    {
        _service = new ConsultaService(_repo.Object, _pets.Object, _audit.Object, new ConsultaRequestValidator());
    }

    private static Pet NovoPet(Guid userId) =>
        new(userId, "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-1), 8m);

    private static ConsultaRequestDto Dto(Guid petId) =>
        new(petId, DateTime.UtcNow.AddDays(5), "Dra. Camila", "Check-up anual");

    [Fact]
    public async Task Criar_Com_Dados_Validos_Deve_Salvar_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pet = NovoPet(userId);
        _pets.Setup(p => p.GetByIdAsync(pet.Id, userId)).ReturnsAsync(pet);

        // Act
        var result = await _service.CriarAsync(userId, Dto(pet.Id));

        // Assert
        result.Veterinario.Should().Be("Dra. Camila");
        result.Status.Should().Be(StatusConsulta.Agendada);
        _repo.Verify(r => r.AddAsync(It.IsAny<Consulta>()), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "CONSULTA_CRIADA")), Times.Once);
    }

    [Fact]
    public async Task Criar_Com_Pet_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _pets.Setup(p => p.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Pet?)null);

        // Act
        var act = () => _service.CriarAsync(Guid.NewGuid(), Dto(Guid.NewGuid()));

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task Criar_Sem_Veterinario_Deve_Lancar_ValidationException()
    {
        // Arrange
        var dto = Dto(Guid.NewGuid()) with { Veterinario = "" };

        // Act
        var act = () => _service.CriarAsync(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task Obter_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Consulta?)null);

        // Act
        var act = () => _service.ObterAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Obter_Existente_Deve_Retornar_Dto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var consulta = new Consulta(Guid.NewGuid(), DateTime.UtcNow.AddDays(2), "Dr. Paulo", "Alergia");
        _repo.Setup(r => r.GetByIdAsync(consulta.Id, userId)).ReturnsAsync(consulta);

        // Act
        var result = await _service.ObterAsync(consulta.Id, userId);

        // Assert
        result.Motivo.Should().Be("Alergia");
        result.Veterinario.Should().Be("Dr. Paulo");
    }

    [Fact]
    public async Task Atualizar_Deve_Alterar_Status_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var petId = Guid.NewGuid();
        var consulta = new Consulta(petId, DateTime.UtcNow.AddDays(2), "Dr. Paulo", "Alergia");
        _repo.Setup(r => r.GetByIdAsync(consulta.Id, userId)).ReturnsAsync(consulta);
        var dto = new ConsultaRequestDto(petId, DateTime.UtcNow, "Dr. Paulo", "Alergia", StatusConsulta.Realizada);

        // Act
        var result = await _service.AtualizarAsync(consulta.Id, userId, dto);

        // Assert
        result.Status.Should().Be(StatusConsulta.Realizada);
        _repo.Verify(r => r.UpdateAsync(consulta), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "CONSULTA_ATUALIZADA")), Times.Once);
    }

    [Fact]
    public async Task Atualizar_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Consulta?)null);

        // Act
        var act = () => _service.AtualizarAsync(Guid.NewGuid(), Guid.NewGuid(), Dto(Guid.NewGuid()));

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task Remover_Existente_Deve_Excluir_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var consulta = new Consulta(Guid.NewGuid(), DateTime.UtcNow.AddDays(2), "Dr. Paulo", "Alergia");
        _repo.Setup(r => r.GetByIdAsync(consulta.Id, userId)).ReturnsAsync(consulta);

        // Act
        await _service.RemoverAsync(consulta.Id, userId);

        // Assert
        _repo.Verify(r => r.DeleteAsync(consulta), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "CONSULTA_REMOVIDA")), Times.Once);
    }

    [Fact]
    public async Task Remover_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Consulta?)null);

        // Act
        var act = () => _service.RemoverAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task Listar_Deve_Retornar_Resultado_Paginado()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IEnumerable<Consulta> lista = new[]
        {
            new Consulta(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), "Dr. Paulo", "Rotina")
        };
        _repo.Setup(r => r.ListAsync(userId, It.IsAny<Guid?>(), It.IsAny<StatusConsulta?>(), It.IsAny<string?>(), 1, 10))
             .ReturnsAsync((lista, 1));

        // Act
        var result = await _service.ListarAsync(userId, new ConsultaQueryParams());

        // Assert
        result.Total.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(1);
    }
}