using PetCareAI.Application.Common;

namespace PetCareAI.Application.DTOs;

public record VacinaRequestDto(Guid PetId, string Nome, DateTime DataAplicacao, DateTime? ProximaDose);
public record VacinaResponseDto(Guid Id, Guid PetId, string Nome, DateTime DataAplicacao, DateTime? ProximaDose, bool Pendente);

public class VacinaQueryParams : QueryParams
{
    public Guid? PetId { get; set; }
}