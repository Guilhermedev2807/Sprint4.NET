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

public interface IConsultaService
{
    Task<PagedResult<ConsultaResponseDto>> ListarAsync(Guid userId, ConsultaQueryParams query);
    Task<ConsultaResponseDto> ObterAsync(Guid id, Guid userId);
    Task<ConsultaResponseDto> CriarAsync(Guid userId, ConsultaRequestDto dto);
    Task<ConsultaResponseDto> AtualizarAsync(Guid id, Guid userId, ConsultaRequestDto dto);
    Task RemoverAsync(Guid id, Guid userId);
}

public interface IVacinaService
{
    Task<PagedResult<VacinaResponseDto>> ListarAsync(Guid userId, VacinaQueryParams query);
    Task<VacinaResponseDto> ObterAsync(Guid id, Guid userId);
    Task<VacinaResponseDto> CriarAsync(Guid userId, VacinaRequestDto dto);
    Task<VacinaResponseDto> AtualizarAsync(Guid id, Guid userId, VacinaRequestDto dto);
    Task RemoverAsync(Guid id, Guid userId);
}