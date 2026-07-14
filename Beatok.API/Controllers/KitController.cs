using Beatok.Application.DTOs.Kit;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("kits")]
    [ApiController]
    [Authorize]
    public class KitController(IKitService kitService) : ControllerBase
    {
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

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid id, [FromBody] UpdateKitDto dto)
        {
            await kitService.UpdateAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await kitService.DeleteAsync(id);
            return Ok();
        }
    }
}
