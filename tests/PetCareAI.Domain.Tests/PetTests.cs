using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Tests;

public class PetTests
{
    [Fact]
    public void Criar_Pet_Valido_Deve_Preencher_Propriedades()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var pet = new Pet(usuarioId, "Rex", "Cachorro", "Labrador", DateTime.UtcNow.AddYears(-2), 12.5m);

        // Assert
        Assert.Equal("Rex", pet.Nome);
        Assert.Equal(usuarioId, pet.UsuarioId);
        Assert.Equal(12.5m, pet.Peso);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_Pet_Com_Peso_Invalido_Deve_Lancar_DomainException(decimal peso)
    {
        // Arrange
        var nascimento = DateTime.UtcNow.AddYears(-1);

        // Act
        var act = () => new Pet(Guid.NewGuid(), "Rex", "Cachorro", null, nascimento, peso);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Criar_Pet_Com_Nascimento_Futuro_Deve_Lancar_DomainException()
    {
        // Arrange
        var futuro = DateTime.UtcNow.AddDays(10);

        // Act
        var act = () => new Pet(Guid.NewGuid(), "Rex", "Cachorro", null, futuro, 5m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void IdadeEmAnos_Deve_Calcular_Corretamente()
    {
        // Arrange
        var pet = new Pet(Guid.NewGuid(), "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-3).AddDays(-5), 8m);

        // Act
        var idade = pet.IdadeEmAnos();

        // Assert
        Assert.Equal(3, idade);
    }

    [Fact]
    public void Atualizar_Deve_Alterar_Dados()
    {
        // Arrange
        var pet = new Pet(Guid.NewGuid(), "Rex", "Cachorro", null, DateTime.UtcNow.AddYears(-1), 8m);

        // Act
        pet.Atualizar("Thor", "Cachorro", "Poodle", DateTime.UtcNow.AddYears(-1), 9m);

        // Assert
        Assert.Equal("Thor", pet.Nome);
        Assert.Equal("Poodle", pet.Raca);
        Assert.Equal(9m, pet.Peso);
    }
}