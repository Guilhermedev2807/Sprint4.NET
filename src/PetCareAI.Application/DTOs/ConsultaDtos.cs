using PetCareAI.Application.Common;
using PetCareAI.Domain.Entities;

namespace PetCareAI.Application.DTOs;

public record ConsultaRequestDto(Guid PetId, DateTime DataHora, string Veterinario, string Motivo, StatusConsulta Status = StatusConsulta.Agendada);
public record ConsultaResponseDto(Guid Id, Guid PetId, DateTime DataHora, string Veterinario, string Motivo, StatusConsulta Status);

public class ConsultaQueryParams : QueryParams
{
    public Guid? PetId { get; set; }
    public StatusConsulta? Status { get; set; }
}