using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyList.Server.DTOs;
using MyList.Server.Services;

namespace MyList.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController(
        ILogger<ItemsController> logger,
        IItemService itemService
        ) : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger = logger;
        private readonly IItemService _itemService = itemService;

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditListAsync(int id, [FromBody] EditItemDTO itemDto)
        {
            _logger.LogInformation("Editting item (ID: {})...", id);
            var item = await _itemService.EditItemAsync(id, itemDto);

            if (item == null)
            {
                _logger.LogWarning("Item (ID: {}) was not edited", id);
            }
            else
            {
                _logger.LogInformation("Item (ID: {}) was edited", item.Id);
            }

            return item == null ? BadRequest() : Ok(item);
        }

        [HttpPost("{id}/move/{listId}")]
        public async Task<IActionResult> MoveItemAsync(int id, int listId)
        {
            _logger.LogInformation("Moving item (ID: {}) to other list (ID: {})...", id, listId);

            var item = await _itemService.MoveItemAsync(id, listId);

            if (item == null)
            {
                _logger.LogWarning("Item (ID: {}) was not moved", id);
            }
            else
            {
                _logger.LogInformation("Item (ID: {}) was moved", item.Id);
            }

            return item == null ? BadRequest() : Ok(item);
        }
    }
}
