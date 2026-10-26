using FluentValidation;
using PetCareAI.Application.Common;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Exceptions;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Application.Services;

public class PetService(
    IPetRepository repo,
    IAuditLogRepository audit,
    IValidator<PetRequestDto> validator) : IPetService
{
    public async Task<PagedResult<PetResponseDto>> ListarAsync(Guid userId, PetQueryParams q)
    {
        var (items, total) = await repo.ListAsync(userId, q.Especie, q.Sort, q.Page, q.PageSize);
        return new PagedResult<PetResponseDto>(items.Select(Map), q.Page, q.PageSize, total);
    }

    public async Task<PetResponseDto> ObterAsync(Guid id, Guid userId) =>
        Map(await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Pet não encontrado."));

    public async Task<PetResponseDto> CriarAsync(Guid userId, PetRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);

        var pet = new Pet(userId, dto.Nome, dto.Especie, dto.Raca, dto.DataNascimento, dto.Peso);
        await repo.AddAsync(pet);
        await Auditar(userId, AuditActions.PetCriado, pet.Id);
        return Map(pet);
    }

    public async Task<PetResponseDto> AtualizarAsync(Guid id, Guid userId, PetRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);

        var pet = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Pet não encontrado.");
        pet.Atualizar(dto.Nome, dto.Especie, dto.Raca, dto.DataNascimento, dto.Peso);
        await repo.UpdateAsync(pet);
        await Auditar(userId, AuditActions.PetAtualizado, pet.Id);
        return Map(pet);
    }

    public async Task RemoverAsync(Guid id, Guid userId)
    {
        var pet = await repo.GetByIdAsync(id, userId) ?? throw new NotFoundException("Pet não encontrado.");
        await repo.DeleteAsync(pet);
        await Auditar(userId, AuditActions.PetRemovido, id);
    }

    private Task Auditar(Guid userId, string acao, Guid petId) =>
        audit.RegistrarAsync(new AuditLog
        {
            UsuarioId = userId,
            Acao = acao,
            Entidade = nameof(Pet),
            Detalhes = petId.ToString()
        });

    private static PetResponseDto Map(Pet p) =>
        new(p.Id, p.Nome, p.Especie, p.Raca, p.DataNascimento, p.IdadeEmAnos(), p.Peso);
}