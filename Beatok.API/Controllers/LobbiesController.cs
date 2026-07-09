using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("lobbies")]
    [ApiController]
    [Authorize]
    [ImplicitAnonymous]
    public class LobbiesController(ILobbyService lobbyService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLobbyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await lobbyService.CreateAsync(dto, Guid.Parse(userId!));
            return Ok();
        }
    }
}
