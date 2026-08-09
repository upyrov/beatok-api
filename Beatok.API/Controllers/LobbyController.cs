using System.Security.Claims;
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
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateLobbyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            var lobbyId = await lobbyService.CreateAsync(dto, userId);
            return Ok(lobbyId);
        }

        [HttpGet]
        public async Task<ActionResult<List<LobbyDto>>> GetAll([FromQuery] LobbyFilterDto filter)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await lobbyService.GetAllAsync(filter, userId));
        }

        [Authorize]
        [HttpPatch("{id:guid}/start")]
        public async Task<IActionResult> Start([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await lobbyService.StartAsync(id, userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:guid}/participants/{targetUserId}")]
        public async Task<IActionResult> Kick([FromRoute] Guid id, [FromRoute] string targetUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await lobbyService.KickAsync(id, userId, targetUserId);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id:guid}/scores/{scoreId:guid}")]
        public async Task<IActionResult> UpdateScore([FromRoute] Guid id, [FromRoute] Guid scoreId, [FromBody] ScoreUpdateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await scoreService.UpdateValueAsync(userId, id, scoreId, dto);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id:guid}/scores")]
        public async Task<ActionResult<Guid>> Vote([FromRoute] Guid id, CreateScoreDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            var scoreId = await scoreService.CreateAsync(userId, id, dto);
            return Ok(scoreId);
        }
    }
}
