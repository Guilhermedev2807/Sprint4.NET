using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;

namespace PetCareAI.API.Controllers;

[ApiController, Authorize, Route("api/consultas")]
public class ConsultasController(IConsultaService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ConsultaQueryParams query)
    {
        var r = await service.ListarAsync(UserId, query);
        var items = r.Items.Select(c => new Resource<ConsultaResponseDto>(c, Links(c)));
        return Ok(new { items, r.Page, r.PageSize, r.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var c = await service.ObterAsync(id, UserId);
        return Ok(new Resource<ConsultaResponseDto>(c, Links(c)));
    }

    [HttpPost]
    public async Task<IActionResult> Criar(ConsultaRequestDto dto)
    {
        var c = await service.CriarAsync(UserId, dto);
        return CreatedAtAction(nameof(Obter), new { id = c.Id }, new Resource<ConsultaResponseDto>(c, Links(c)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, ConsultaRequestDto dto) =>
        Ok(await service.AtualizarAsync(id, UserId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        await service.RemoverAsync(id, UserId);
        return NoContent();
    }

    private static IEnumerable<Link> Links(ConsultaResponseDto c) => new[]
    {
        new Link($"/api/consultas/{c.Id}", "self", "GET"),
        new Link($"/api/consultas/{c.Id}", "update", "PUT"),
        new Link($"/api/consultas/{c.Id}", "delete", "DELETE"),
        new Link($"/api/pets/{c.PetId}", "pet", "GET")
    };
}