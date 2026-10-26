using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Entities;

public class Vacina
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PetId { get; private set; }
    public string Nome { get; private set; } = default!;
    public DateTime DataAplicacao { get; private set; }
    public DateTime? ProximaDose { get; private set; }

    protected Vacina() { }

    public Vacina(Guid petId, string nome, DateTime dataAplicacao, DateTime? proximaDose)
    {
        Validar(nome, dataAplicacao, proximaDose);
        PetId = petId;
        Nome = nome.Trim();
        DataAplicacao = dataAplicacao;
        ProximaDose = proximaDose;
    }

    public void Atualizar(string nome, DateTime dataAplicacao, DateTime? proximaDose)
    {
        Validar(nome, dataAplicacao, proximaDose);
        Nome = nome.Trim();
        DataAplicacao = dataAplicacao;
        ProximaDose = proximaDose;
    }

    public bool EstaPendente() => ProximaDose.HasValue && ProximaDose.Value <= DateTime.UtcNow.AddDays(30);

    private static void Validar(string nome, DateTime aplicacao, DateTime? proxima)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome da vacina é obrigatório.");
        if (proxima.HasValue && proxima.Value < aplicacao)
            throw new DomainException("Próxima dose não pode ser anterior à aplicação.");
    }
}