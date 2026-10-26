using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string SenhaHash { get; private set; } = default!;
    public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;
    public ICollection<Pet> Pets { get; private set; } = new List<Pet>();

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new DomainException("E-mail inválido.");
        if (string.IsNullOrWhiteSpace(senhaHash)) throw new DomainException("Senha é obrigatória.");

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
    }
}