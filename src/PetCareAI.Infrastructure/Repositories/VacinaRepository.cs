using Microsoft.EntityFrameworkCore;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Persistence;

namespace PetCareAI.Infrastructure.Repositories;

public class VacinaRepository(AppDbContext db) : IVacinaRepository
{
    private IQueryable<Vacina> DoUsuario(Guid usuarioId) =>
        db.Vacinas.Where(v => db.Pets.Any(p => p.Id == v.PetId && p.UsuarioId == usuarioId));

    public Task<Vacina?> GetByIdAsync(Guid id, Guid usuarioId) =>
        DoUsuario(usuarioId).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<(IEnumerable<Vacina> Items, int Total)> ListAsync(
        Guid usuarioId, Guid? petId, string? sort, int page, int pageSize)
    {
        var q = DoUsuario(usuarioId).AsNoTracking();

        if (petId.HasValue) q = q.Where(v => v.PetId == petId);

        q = sort?.ToLowerInvariant() switch
        {
            "nome" => q.OrderBy(v => v.Nome),
            "-nome" => q.OrderByDescending(v => v.Nome),
            "proxima" => q.OrderBy(v => v.ProximaDose),
            _ => q.OrderBy(v => v.DataAplicacao)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task AddAsync(Vacina v) { db.Vacinas.Add(v); await db.SaveChangesAsync(); }
    public async Task UpdateAsync(Vacina v) { db.Vacinas.Update(v); await db.SaveChangesAsync(); }
    public async Task DeleteAsync(Vacina v) { db.Vacinas.Remove(v); await db.SaveChangesAsync(); }
}