using PetCareAI.Application.Common;
using PetCareAI.Application.DTOs;

namespace PetCareAI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public interface IPetService
{
    Task<PagedResult<PetResponseDto>> ListarAsync(Guid userId, PetQueryParams query);
    Task<PetResponseDto> ObterAsync(Guid id, Guid userId);
    Task<PetResponseDto> CriarAsync(Guid userId, PetRequestDto dto);
    Task<PetResponseDto> AtualizarAsync(Guid id, Guid userId, PetRequestDto dto);
    Task RemoverAsync(Guid id, Guid userId);
}