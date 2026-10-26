namespace PetCareAI.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UsuarioId { get; set; }
    public string Acao { get; set; } = default!;
    public string Entidade { get; set; } = default!;
    public string? Detalhes { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}