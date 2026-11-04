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

public class VacinaServiceTests
{
    private readonly Mock<IVacinaRepository> _repo = new();
    private readonly Mock<IPetRepository> _pets = new();
    private readonly Mock<IAuditLogRepository> _audit = new();
    private readonly VacinaService _service;

    public VacinaServiceTests()
    {
        _service = new VacinaService(_repo.Object, _pets.Object, _audit.Object, new VacinaRequestValidator());
    }

    private static Pet NovoPet(Guid userId) =>
        new(userId, "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-1), 8m);

    private static VacinaRequestDto Dto(Guid petId) =>
        new(petId, "V10", DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddMonths(11));

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
        result.Nome.Should().Be("V10");
        result.PetId.Should().Be(pet.Id);
        _repo.Verify(r => r.AddAsync(It.IsAny<Vacina>()), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "VACINA_CRIADA")), Times.Once);
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
        _repo.Verify(r => r.AddAsync(It.IsAny<Vacina>()), Times.Never);
    }

    [Fact]
    public async Task Criar_Sem_Nome_Deve_Lancar_ValidationException()
    {
        // Arrange
        var dto = Dto(Guid.NewGuid()) with { Nome = "" };

        // Act
        var act = () => _service.CriarAsync(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Vacina>()), Times.Never);
    }

    [Fact]
    public async Task Criar_Com_Proxima_Dose_Anterior_A_Aplicacao_Deve_Lancar_DomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pet = NovoPet(userId);
        _pets.Setup(p => p.GetByIdAsync(pet.Id, userId)).ReturnsAsync(pet);
        var dto = new VacinaRequestDto(pet.Id, "V10", DateTime.UtcNow, DateTime.UtcNow.AddDays(-30));

        // Act
        var act = () => _service.CriarAsync(userId, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Vacina>()), Times.Never);
    }

    [Fact]
    public async Task Obter_Com_Proxima_Dose_Em_10_Dias_Deve_Estar_Pendente()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var vacina = new Vacina(Guid.NewGuid(), "Antirrábica", DateTime.UtcNow.AddMonths(-11), DateTime.UtcNow.AddDays(10));
        _repo.Setup(r => r.GetByIdAsync(vacina.Id, userId)).ReturnsAsync(vacina);

        // Act
        var result = await _service.ObterAsync(vacina.Id, userId);

        // Assert
        result.Pendente.Should().BeTrue();
    }

    [Fact]
    public async Task Obter_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Vacina?)null);

        // Act
        var act = () => _service.ObterAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Atualizar_Deve_Alterar_Dados_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var petId = Guid.NewGuid();
        var vacina = new Vacina(petId, "V8", DateTime.UtcNow.AddMonths(-2), null);
        _repo.Setup(r => r.GetByIdAsync(vacina.Id, userId)).ReturnsAsync(vacina);
        var dto = new VacinaRequestDto(petId, "V10", DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddMonths(11));

        // Act
        var result = await _service.AtualizarAsync(vacina.Id, userId, dto);

        // Assert
        result.Nome.Should().Be("V10");
        _repo.Verify(r => r.UpdateAsync(vacina), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "VACINA_ATUALIZADA")), Times.Once);
    }

    [Fact]
    public async Task Remover_Existente_Deve_Excluir_E_Auditar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var vacina = new Vacina(Guid.NewGuid(), "V10", DateTime.UtcNow.AddMonths(-1), null);
        _repo.Setup(r => r.GetByIdAsync(vacina.Id, userId)).ReturnsAsync(vacina);

        // Act
        await _service.RemoverAsync(vacina.Id, userId);

        // Assert
        _repo.Verify(r => r.DeleteAsync(vacina), Times.Once);
        _audit.Verify(a => a.RegistrarAsync(It.Is<AuditLog>(l => l.Acao == "VACINA_REMOVIDA")), Times.Once);
    }

    [Fact]
    public async Task Remover_Inexistente_Deve_Lancar_NotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Vacina?)null);

        // Act
        var act = () => _service.RemoverAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Vacina>()), Times.Never);
    }

    [Fact]
    public async Task Listar_Deve_Retornar_Resultado_Paginado()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IEnumerable<Vacina> lista = new[]
        {
            new Vacina(Guid.NewGuid(), "V10", DateTime.UtcNow.AddMonths(-1), null)
        };
        _repo.Setup(r => r.ListAsync(userId, It.IsAny<Guid?>(), It.IsAny<string?>(), 1, 10))
             .ReturnsAsync((lista, 1));

        // Act
        var result = await _service.ListarAsync(userId, new VacinaQueryParams());

        // Assert
        result.Total.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}