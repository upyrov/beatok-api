using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("lobbies")]
    [ApiController]
    public class LobbyController(ILobbyService lobbyService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [ImplicitAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateLobbyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await lobbyService.CreateAsync(dto, Guid.Parse(userId!));
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] LobbyFilterDto filter)
        {
            return Ok(await lobbyService.GetAllAsync(filter));
        }

        [Authorize]
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartLobby([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lobbyService.StartAsync(id, Guid.Parse(userId!));
            return Ok();
        }
    }
}
