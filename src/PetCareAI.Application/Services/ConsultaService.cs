using FluentValidation;
using PetCareAI.Application.Common;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Services;

public class ConsultaService(
    IConsultaRepository repo,
    IPetRepository pets,
    IAuditLogRepository audit,
    IValidator<ConsultaRequestDto> validator) : IConsultaService
{
    public async Task<PagedResult<ConsultaResponseDto>> ListarAsync(Guid userId, ConsultaQueryParams q)
    {
        var (items, total) = await repo.ListAsync(userId, q.PetId, q.Status, q.Sort, q.Page, q.PageSize);
        return new PagedResult<ConsultaResponseDto>(items.Select(Map), q.Page, q.PageSize, total);
    }

    public async Task<ConsultaResponseDto> ObterAsync(Guid id, Guid userId) =>
        Map(await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Consulta não encontrada."));

    public async Task<ConsultaResponseDto> CriarAsync(Guid userId, ConsultaRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);
        _ = await pets.GetByIdAsync(dto.PetId, userId) ?? throw new NotFoundException("Pet não encontrado.");

        var consulta = new Consulta(dto.PetId, dto.DataHora, dto.Veterinario, dto.Motivo);
        await repo.AddAsync(consulta);
        await Auditar(userId, "CONSULTA_CRIADA", consulta.Id);
        return Map(consulta);
    }

    public async Task<ConsultaResponseDto> AtualizarAsync(Guid id, Guid userId, ConsultaRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);
        var consulta = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Consulta não encontrada.");
        consulta.Atualizar(dto.DataHora, dto.Veterinario, dto.Motivo, dto.Status);
        await repo.UpdateAsync(consulta);
        await Auditar(userId, "CONSULTA_ATUALIZADA", consulta.Id);
        return Map(consulta);
    }

    public async Task RemoverAsync(Guid id, Guid userId)
    {
        var consulta = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Consulta não encontrada.");
        await repo.DeleteAsync(consulta);
        await Auditar(userId, "CONSULTA_REMOVIDA", id);
    }

    private Task Auditar(Guid userId, string acao, Guid id) =>
        audit.RegistrarAsync(new AuditLog { UsuarioId = userId, Acao = acao, Entidade = nameof(Consulta), Detalhes = id.ToString() });

    private static ConsultaResponseDto Map(Consulta c) =>
        new(c.Id, c.PetId, c.DataHora, c.Veterinario, c.Motivo, c.Status);
}