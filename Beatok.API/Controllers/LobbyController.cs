using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.Score;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("lobbies")]
    [ApiController]
    public class LobbyController(ILobbyService lobbyService, 
        IScoreService scoreService) : ControllerBase
    {
        [Authorize]
        [AnonymousAuthorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateLobbyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var lobbyId = await lobbyService.CreateAsync(dto, Guid.Parse(userId!));
            return Ok(lobbyId);
        }

        [HttpGet]
        public async Task<ActionResult<List<LobbyDto>>> GetAll([FromQuery] LobbyFilterDto filter)
        {
            return Ok(await lobbyService.GetAllAsync(filter));
        }

        [Authorize]
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> Start([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lobbyService.StartAsync(id, Guid.Parse(userId!));
            return Ok();
        }

        [Authorize]
        [AnonymousAuthorize]
        [HttpPost("{id}/participants")]
        public async Task<IActionResult> Join([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lobbyService.JoinAsync(id, Guid.Parse(userId!));
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}/participants/me")]
        public async Task<IActionResult> Leave([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lobbyService.LeaveAsync(id, Guid.Parse(userId!));
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/scores")]
        public async Task<IActionResult> Vote([FromRoute] Guid id, CreateScoreDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await scoreService.CreateAsync(Guid.Parse(userId!), id, dto);
            return Ok();
        }
    }
}
