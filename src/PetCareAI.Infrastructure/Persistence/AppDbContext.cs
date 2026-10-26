using Microsoft.EntityFrameworkCore;
using PetCareAI.Domain.Entities;

namespace PetCareAI.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Usuario>(e =>
        {
            e.ToTable("USUARIO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasMaxLength(160).IsRequired();
            e.Property(x => x.SenhaHash).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        mb.Entity<Pet>(e =>
        {
            e.ToTable("PET");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Especie).HasMaxLength(50).IsRequired();
            e.Property(x => x.Raca).HasMaxLength(80);
            e.Property(x => x.Peso).HasPrecision(6, 2);
            e.HasOne<Usuario>().WithMany(u => u.Pets).HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.UsuarioId);
        });

        mb.Entity<Consulta>(e =>
        {
            e.ToTable("CONSULTA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Veterinario).HasMaxLength(120).IsRequired();
            e.Property(x => x.Motivo).HasMaxLength(300).IsRequired();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasOne<Pet>().WithMany(p => p.Consultas).HasForeignKey(c => c.PetId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.PetId);
        });

        mb.Entity<Vacina>(e =>
        {
            e.ToTable("VACINA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            e.HasOne<Pet>().WithMany(p => p.Vacinas).HasForeignKey(v => v.PetId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.PetId);
        });
    }
}