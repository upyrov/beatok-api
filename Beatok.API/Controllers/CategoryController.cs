using Beatok.Application.DTOs.Category;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("categories")]
    [ApiController]
    [Authorize]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            await categoryService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid id, [FromBody] UpdateCategoryDto dto)
        {
            await categoryService.UpdateNameAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await categoryService.DeleteAsync(id);
            return Ok();
        }
    }
}
