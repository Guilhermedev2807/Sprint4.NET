using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareAI.Application.DTOs;
using PetCareAI.Application.Interfaces;

namespace PetCareAI.API.Controllers;

[ApiController, AllowAnonymous, Route("api/auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto) =>
        StatusCode(StatusCodes.Status201Created, await service.RegistrarAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto) => Ok(await service.LoginAsync(dto));
}