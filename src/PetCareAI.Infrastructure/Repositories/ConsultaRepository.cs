using Microsoft.EntityFrameworkCore;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Persistence;

namespace PetCareAI.Infrastructure.Repositories;

public class ConsultaRepository(AppDbContext db) : IConsultaRepository
{
    private IQueryable<Consulta> DoUsuario(Guid usuarioId) =>
        db.Consultas.Where(c => db.Pets.Any(p => p.Id == c.PetId && p.UsuarioId == usuarioId));

    public Task<Consulta?> GetByIdAsync(Guid id, Guid usuarioId) =>
        DoUsuario(usuarioId).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<(IEnumerable<Consulta> Items, int Total)> ListAsync(
        Guid usuarioId, Guid? petId, StatusConsulta? status, string? sort, int page, int pageSize)
    {
        var q = DoUsuario(usuarioId).AsNoTracking();

        if (petId.HasValue) q = q.Where(c => c.PetId == petId);
        if (status.HasValue) q = q.Where(c => c.Status == status);

        q = sort?.ToLowerInvariant() switch
        {
            "data" => q.OrderBy(c => c.DataHora),
            "-data" => q.OrderByDescending(c => c.DataHora),
            _ => q.OrderBy(c => c.DataHora)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task AddAsync(Consulta c) { db.Consultas.Add(c); await db.SaveChangesAsync(); }
    public async Task UpdateAsync(Consulta c) { db.Consultas.Update(c); await db.SaveChangesAsync(); }
    public async Task DeleteAsync(Consulta c) { db.Consultas.Remove(c); await db.SaveChangesAsync(); }
}