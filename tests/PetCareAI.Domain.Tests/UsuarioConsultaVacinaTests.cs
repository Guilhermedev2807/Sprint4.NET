using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Tests;

public class UsuarioConsultaVacinaTests
{
    [Fact]
    public void Usuario_Com_Email_Invalido_Deve_Lancar_DomainException()
    {
        // Arrange & Act
        var act = () => new Usuario("Ana", "emailinvalido", "hash");

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Usuario_Deve_Normalizar_Email()
    {
        // Arrange & Act
        var u = new Usuario("Ana", "  ANA@Teste.com ", "hash");

        // Assert
        Assert.Equal("ana@teste.com", u.Email);
    }

    [Fact]
    public void Consulta_Realizada_Nao_Pode_Ser_Cancelada()
    {
        // Arrange
        var c = new Consulta(Guid.NewGuid(), DateTime.UtcNow, "Dra. Lia", "Rotina");
        c.Atualizar(DateTime.UtcNow, "Dra. Lia", "Rotina", StatusConsulta.Realizada);

        // Act
        var act = () => c.Cancelar();

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Consulta_Agendada_Pode_Ser_Cancelada()
    {
        // Arrange
        var c = new Consulta(Guid.NewGuid(), DateTime.UtcNow.AddDays(2), "Dra. Lia", "Rotina");

        // Act
        c.Cancelar();

        // Assert
        Assert.Equal(StatusConsulta.Cancelada, c.Status);
    }

    [Fact]
    public void Vacina_Com_Proxima_Dose_Anterior_Deve_Lancar_DomainException()
    {
        // Arrange
        var aplicacao = DateTime.UtcNow;

        // Act
        var act = () => new Vacina(Guid.NewGuid(), "V10", aplicacao, aplicacao.AddDays(-1));

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Vacina_Com_Proxima_Dose_Proxima_Deve_Estar_Pendente()
    {
        // Arrange
        var v = new Vacina(Guid.NewGuid(), "Antirrábica", DateTime.UtcNow.AddMonths(-11), DateTime.UtcNow.AddDays(10));

        // Act
        var pendente = v.EstaPendente();

        // Assert
        Assert.True(pendente);
    }
}