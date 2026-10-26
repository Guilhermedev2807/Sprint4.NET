using Microsoft.EntityFrameworkCore;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;
using PetCareAI.Infrastructure.Persistence;

namespace PetCareAI.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> GetByIdAsync(Guid id) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

    public Task<Usuario?> GetByEmailAsync(string email) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task AddAsync(Usuario usuario)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
    }
}