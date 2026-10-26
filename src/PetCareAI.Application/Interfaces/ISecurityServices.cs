using PetCareAI.Domain.Entities;

namespace PetCareAI.Application.Interfaces;

public interface ITokenService
{
    string Gerar(Usuario usuario);
}

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}