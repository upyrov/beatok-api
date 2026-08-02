using Beatok.Application.DTOs.Kit;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("kits")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public class KitController(IKitService kitService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKitDto dto)
        {
            await kitService.CreateAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<KitDto>>> GetAll()
        {
            return Ok(await kitService.GetAllAsync());
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateName([FromRoute] Guid id, [FromBody] UpdateKitDto dto)
        {
            await kitService.UpdateAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await kitService.DeleteAsync(id);
            return Ok();
        }
    }
}
