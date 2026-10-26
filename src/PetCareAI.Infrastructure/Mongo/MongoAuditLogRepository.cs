using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PetCareAI.Domain.Entities;
using PetCareAI.Domain.Interfaces;

namespace PetCareAI.Infrastructure.Mongo;

internal class AuditLogDocument
{
    [BsonId, BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid? UsuarioId { get; set; }

    public string Acao { get; set; } = default!;
    public string Entidade { get; set; } = default!;
    public string? Detalhes { get; set; }
    public DateTime Data { get; set; }
}

public class MongoAuditLogRepository(IMongoClient client) : IAuditLogRepository
{
    private readonly IMongoCollection<AuditLogDocument> _col =
        client.GetDatabase("petcare").GetCollection<AuditLogDocument>("audit_logs");

    public Task RegistrarAsync(AuditLog log) =>
        _col.InsertOneAsync(new AuditLogDocument
        {
            Id = log.Id,
            UsuarioId = log.UsuarioId,
            Acao = log.Acao,
            Entidade = log.Entidade,
            Detalhes = log.Detalhes,
            Data = log.Data
        });
}