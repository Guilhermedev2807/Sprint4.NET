using FluentValidation;
using PetCareAI.Application.Common;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Services;

public class VacinaService(
    IVacinaRepository repo,
    IPetRepository pets,
    IAuditLogRepository audit,
    IValidator<VacinaRequestDto> validator) : IVacinaService
{
    public async Task<PagedResult<VacinaResponseDto>> ListarAsync(Guid userId, VacinaQueryParams q)
    {
        var (items, total) = await repo.ListAsync(userId, q.PetId, q.Sort, q.Page, q.PageSize);
        return new PagedResult<VacinaResponseDto>(items.Select(Map), q.Page, q.PageSize, total);
    }

    public async Task<VacinaResponseDto> ObterAsync(Guid id, Guid userId) =>
        Map(await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Vacina não encontrada."));

    public async Task<VacinaResponseDto> CriarAsync(Guid userId, VacinaRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);
        _ = await pets.GetByIdAsync(dto.PetId, userId) ?? throw new NotFoundException("Pet não encontrado.");

        var vacina = new Vacina(dto.PetId, dto.Nome, dto.DataAplicacao, dto.ProximaDose);
        await repo.AddAsync(vacina);
        await Auditar(userId, "VACINA_CRIADA", vacina.Id);
        return Map(vacina);
    }

    public async Task<VacinaResponseDto> AtualizarAsync(Guid id, Guid userId, VacinaRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);
        var vacina = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Vacina não encontrada.");
        vacina.Atualizar(dto.Nome, dto.DataAplicacao, dto.ProximaDose);
        await repo.UpdateAsync(vacina);
        await Auditar(userId, "VACINA_ATUALIZADA", vacina.Id);
        return Map(vacina);
    }

    public async Task RemoverAsync(Guid id, Guid userId)
    {
        var vacina = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Vacina não encontrada.");
        await repo.DeleteAsync(vacina);
        await Auditar(userId, "VACINA_REMOVIDA", id);
    }

    private Task Auditar(Guid userId, string acao, Guid id) =>
        audit.RegistrarAsync(new AuditLog { UsuarioId = userId, Acao = acao, Entidade = nameof(Vacina), Detalhes = id.ToString() });

    private static VacinaResponseDto Map(Vacina v) =>
        new(v.Id, v.PetId, v.Nome, v.DataAplicacao, v.ProximaDose, v.EstaPendente());
}