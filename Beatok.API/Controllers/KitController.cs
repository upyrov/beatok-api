using Beatok.Application.DTOs.Kit;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("kits")]
    [ApiController]
    [Authorize]
    public class KitController(IKitService kitService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKitDto dto)
        {
            await kitService.CreateAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await kitService.GetAllAsync());
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid id, [FromBody] UpdateKitDto dto)
        {
            await kitService.UpdateAsync(id, dto);
            return Ok();
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await kitService.DeleteAsync(id);
            return Ok();
        }
    }
}
