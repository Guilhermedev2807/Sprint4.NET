using Microsoft.EntityFrameworkCore;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Persistence;

namespace PetCareAI.Infrastructure.Repositories;

public class PetRepository(AppDbContext db) : IPetRepository
{
    public Task<Pet?> GetByIdAsync(Guid id, Guid usuarioId) =>
        db.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

    public async Task<(IEnumerable<Pet> Items, int Total)> ListAsync(
        Guid usuarioId, string? especie, string? sort, int page, int pageSize)
    {
        var q = db.Pets.AsNoTracking().Where(p => p.UsuarioId == usuarioId);

        if (!string.IsNullOrWhiteSpace(especie))
            q = q.Where(p => p.Especie.ToLower() == especie.ToLower());

        q = sort?.ToLowerInvariant() switch
        {
            "nome" => q.OrderBy(p => p.Nome),
            "-nome" => q.OrderByDescending(p => p.Nome),
            "peso" => q.OrderBy(p => p.Peso),
            "-peso" => q.OrderByDescending(p => p.Peso),
            _ => q.OrderBy(p => p.Nome)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task AddAsync(Pet pet) { db.Pets.Add(pet); await db.SaveChangesAsync(); }
    public async Task UpdateAsync(Pet pet) { db.Pets.Update(pet); await db.SaveChangesAsync(); }
    public async Task DeleteAsync(Pet pet) { db.Pets.Remove(pet); await db.SaveChangesAsync(); }
}