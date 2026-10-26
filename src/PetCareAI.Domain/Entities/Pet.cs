using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Entities;

public class Pet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public string Nome { get; private set; } = default!;
    public string Especie { get; private set; } = default!;
    public string? Raca { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public decimal Peso { get; private set; }
    public ICollection<Consulta> Consultas { get; private set; } = new List<Consulta>();
    public ICollection<Vacina> Vacinas { get; private set; } = new List<Vacina>();

    protected Pet() { }

    public Pet(Guid usuarioId, string nome, string especie, string? raca, DateTime dataNascimento, decimal peso)
    {
        Validar(nome, especie, dataNascimento, peso);
        UsuarioId = usuarioId;
        Nome = nome.Trim();
        Especie = especie.Trim();
        Raca = raca?.Trim();
        DataNascimento = dataNascimento;
        Peso = peso;
    }

    public void Atualizar(string nome, string especie, string? raca, DateTime dataNascimento, decimal peso)
    {
        Validar(nome, especie, dataNascimento, peso);
        Nome = nome.Trim();
        Especie = especie.Trim();
        Raca = raca?.Trim();
        DataNascimento = dataNascimento;
        Peso = peso;
    }

    public int IdadeEmAnos() => (int)((DateTime.UtcNow - DataNascimento).TotalDays / 365.25);

    private static void Validar(string nome, string especie, DateTime nasc, decimal peso)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome do pet é obrigatório.");
        if (string.IsNullOrWhiteSpace(especie)) throw new DomainException("Espécie é obrigatória.");
        if (peso <= 0) throw new DomainException("Peso deve ser maior que zero.");
        if (nasc > DateTime.UtcNow) throw new DomainException("Data de nascimento inválida.");
    }
}