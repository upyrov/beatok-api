using Beatok.API.Attributes;
using Beatok.Application.DTOs.Category;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("categories")]
    [ApiController]
    [Admin]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            await categoryService.CreateAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllByKitId([FromQuery] Guid id)
        {
            var categories = await categoryService.GetAllByKitIdAsync(id);
            return Ok(categories);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateName([FromRoute] Guid id, [FromBody] CategoryUpdateDto dto)
        {
            await categoryService.UpdateNameAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await categoryService.DeleteAsync(id);
            return Ok();
        }
    }
}
