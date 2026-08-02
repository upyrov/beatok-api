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
            if (userId is null)
            {
                return Unauthorized();
            }
            var lobbyId = await lobbyService.CreateAsync(dto, Guid.Parse(userId));
            return Ok(lobbyId);
        }

        [HttpGet]
        public async Task<ActionResult<List<LobbyDto>>> GetAll([FromQuery] LobbyFilterDto filter)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await lobbyService.GetAllAsync(filter, userId));
        }

        [HttpGet("history/{id:guid}")]
        public Task<ActionResult<PageResult<LobbyDto>>> GetAllByUserId([FromRoute] Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return lobbyService.GetAllByUserId(id, page, pageSize)
                .ContinueWith<ActionResult<PageResult<LobbyDto>>>(t => Ok(t.Result));
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
            await lobbyService.StartAsync(id, Guid.Parse(userId));
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:guid}/participants/{targetUserId:guid}")]
        public async Task<IActionResult> Kick([FromRoute] Guid id, [FromRoute] Guid targetUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await lobbyService.KickAsync(id, Guid.Parse(userId), targetUserId);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id:guid}/scores/{scoreId:guid}")]
        public async Task<IActionResult> UpdateScore([FromRoute] Guid id, [FromRoute] Guid scoreId, [FromBody] UpdateScoreDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await scoreService.UpdateValueAsync(Guid.Parse(userId), id, scoreId, dto);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id:guid}/scores")]
        public async Task<IActionResult> Vote([FromRoute] Guid id, CreateScoreDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await scoreService.CreateAsync(Guid.Parse(userId), id, dto);
            return Ok();
        }
    }
}
