using PetCareAI.Application.Common;

namespace PetCareAI.Application.DTOs;

public record PetRequestDto(string Nome, string Especie, string? Raca, DateTime DataNascimento, decimal Peso);
public record PetResponseDto(Guid Id, string Nome, string Especie, string? Raca, DateTime DataNascimento, int Idade, decimal Peso);

public class PetQueryParams : QueryParams
{
    public string? Especie { get; set; }
}