using PetCareAI.Domain.Exceptions;

namespace PetCareAI.Domain.Entities;

public enum StatusConsulta { Agendada = 1, Realizada = 2, Cancelada = 3 }

public class Consulta
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PetId { get; private set; }
    public DateTime DataHora { get; private set; }
    public string Veterinario { get; private set; } = default!;
    public string Motivo { get; private set; } = default!;
    public StatusConsulta Status { get; private set; } = StatusConsulta.Agendada;

    protected Consulta() { }

    public Consulta(Guid petId, DateTime dataHora, string veterinario, string motivo)
    {
        Validar(veterinario, motivo);
        PetId = petId;
        DataHora = dataHora;
        Veterinario = veterinario.Trim();
        Motivo = motivo.Trim();
    }

    public void Atualizar(DateTime dataHora, string veterinario, string motivo, StatusConsulta status)
    {
        Validar(veterinario, motivo);
        DataHora = dataHora;
        Veterinario = veterinario.Trim();
        Motivo = motivo.Trim();
        Status = status;
    }

    public void Cancelar()
    {
        if (Status == StatusConsulta.Realizada)
            throw new DomainException("Consulta já realizada não pode ser cancelada.");
        Status = StatusConsulta.Cancelada;
    }

    private static void Validar(string veterinario, string motivo)
    {
        if (string.IsNullOrWhiteSpace(veterinario)) throw new DomainException("Veterinário é obrigatório.");
        if (string.IsNullOrWhiteSpace(motivo)) throw new DomainException("Motivo é obrigatório.");
    }
}