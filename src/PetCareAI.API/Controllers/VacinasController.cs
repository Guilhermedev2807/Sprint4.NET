using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;

namespace PetCareAI.API.Controllers;

[ApiController, Authorize, Route("api/vacinas")]
public class VacinasController(IVacinaService service) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] VacinaQueryParams query)
    {
        var r = await service.ListarAsync(UserId, query);
        var items = r.Items.Select(v => new Resource<VacinaResponseDto>(v, Links(v)));
        return Ok(new { items, r.Page, r.PageSize, r.Total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var v = await service.ObterAsync(id, UserId);
        return Ok(new Resource<VacinaResponseDto>(v, Links(v)));
    }

    [HttpPost]
    public async Task<IActionResult> Criar(VacinaRequestDto dto)
    {
        var v = await service.CriarAsync(UserId, dto);
        return CreatedAtAction(nameof(Obter), new { id = v.Id }, new Resource<VacinaResponseDto>(v, Links(v)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, VacinaRequestDto dto) =>
        Ok(await service.AtualizarAsync(id, UserId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        await service.RemoverAsync(id, UserId);
        return NoContent();
    }

    private static IEnumerable<Link> Links(VacinaResponseDto v) => new[]
    {
        new Link($"/api/vacinas/{v.Id}", "self", "GET"),
        new Link($"/api/vacinas/{v.Id}", "update", "PUT"),
        new Link($"/api/vacinas/{v.Id}", "delete", "DELETE"),
        new Link($"/api/pets/{v.PetId}", "pet", "GET")
    };
}