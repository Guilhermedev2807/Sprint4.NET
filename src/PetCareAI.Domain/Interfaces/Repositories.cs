using PetCareAI.Domain.Entities;

namespace PetCareAI.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task AddAsync(Usuario usuario);
}

public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(Guid id, Guid usuarioId);
    Task<(IEnumerable<Pet> Items, int Total)> ListAsync(Guid usuarioId, string? especie, string? sort, int page, int pageSize);
    Task AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Pet pet);
}

public interface IConsultaRepository
{
    Task<Consulta?> GetByIdAsync(Guid id, Guid usuarioId);
    Task<(IEnumerable<Consulta> Items, int Total)> ListAsync(Guid usuarioId, Guid? petId, StatusConsulta? status, string? sort, int page, int pageSize);
    Task AddAsync(Consulta consulta);
    Task UpdateAsync(Consulta consulta);
    Task DeleteAsync(Consulta consulta);
}

public interface IVacinaRepository
{
    Task<Vacina?> GetByIdAsync(Guid id, Guid usuarioId);
    Task<(IEnumerable<Vacina> Items, int Total)> ListAsync(Guid usuarioId, Guid? petId, string? sort, int page, int pageSize);
    Task AddAsync(Vacina vacina);
    Task UpdateAsync(Vacina vacina);
    Task DeleteAsync(Vacina vacina);
}

public interface IAuditLogRepository
{
    Task RegistrarAsync(AuditLog log);
}