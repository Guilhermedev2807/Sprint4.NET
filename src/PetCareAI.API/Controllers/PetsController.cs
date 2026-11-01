using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;

namespace PetCareAI.API.Controllers;

public record Link(string Href, string Rel, string Method);
public record Resource<T>(T Data, IEnumerable<Link> Links);

[ApiController, Authorize, Route("api/pets")]
public class PetsController(IPetService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] PetQueryParams query)
    {
        var r = await service.ListarAsync(UserId, query);
        var items = r.Items.Select(p => new Resource<PetResponseDto>(p, LinksPet(p.Id)));
        return Ok(new { items, r.Page, r.PageSize, r.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var pet = await service.ObterAsync(id, UserId);
        return Ok(new Resource<PetResponseDto>(pet, LinksPet(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Criar(PetRequestDto dto)
    {
        var pet = await service.CriarAsync(UserId, dto);
        return CreatedAtAction(nameof(Obter), new { id = pet.Id }, new Resource<PetResponseDto>(pet, LinksPet(pet.Id)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, PetRequestDto dto) =>
        Ok(await service.AtualizarAsync(id, UserId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        await service.RemoverAsync(id, UserId);
        return NoContent();
    }

    private static IEnumerable<Link> LinksPet(Guid id) => new[]
    {
        new Link($"/api/pets/{id}", "self", "GET"),
        new Link($"/api/pets/{id}", "update", "PUT"),
        new Link($"/api/pets/{id}", "delete", "DELETE"),
        new Link($"/api/consultas?petId={id}", "consultas", "GET"),
        new Link($"/api/vacinas?petId={id}", "vacinas", "GET")
    };
}