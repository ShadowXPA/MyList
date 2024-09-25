using Microsoft.AspNetCore.Mvc;
using MyList.Server.DTOs;
using MyList.Server.Models;
using MyList.Server.Services;

namespace MyList.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController(
        ILogger<ListsController> logger,
        IListService listService
        ) : ControllerBase
    {
        private readonly ILogger<ListsController> _logger = logger;
        private readonly IListService _listService = listService;

        [HttpGet]
        public async Task<IActionResult> GetListsAsync()
        {
            _logger.LogInformation("Getting all lists...");
            var lists = await _listService.GetListsAsync();
            _logger.LogInformation("Found {} lists", lists.Count());
            return Ok(lists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListAsync(int id, [FromQuery(Name = "include-items")] bool includeItems = false)
        {
            _logger.LogInformation("Getting list (ID: {}) including items? {}...", id, includeItems);
            var list = await _listService.GetListAsync(id, includeItems);
            _logger.LogInformation("List (ID: {}) was {}", id, (list == null ? "not found" : "found"));
            return list == null ? NotFound() : Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateListAsync([FromBody] CreateListDTO listDto)
        {
            _logger.LogInformation("Creating new list...");
            var newList = await _listService.CreateListAsync(listDto);

            if (newList == null)
            {
                _logger.LogWarning("New list (Name: {}) was not created", listDto.Name);
            }
            else
            {
                _logger.LogInformation("New list (ID: {}) was created", newList.Id);
            }

            return newList == null ? BadRequest() : Ok(newList);
        }

        [HttpPatch("{id}")]
        public Task EditListAsync(int id, [FromBody] EditListDTO listDto)
        {
            _logger.LogInformation("Editting list (ID: {})...", id);
            return _listService.EditListAsync(id, listDto);
        }

        [HttpDelete("{id}")]
        public Task DeleteListAsync(int id)
        {
            _logger.LogInformation("Deleting list (ID: {})...", id);
            return _listService.DeleteListAsync(id);
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItemsAsync(int id)
        {
            _logger.LogInformation("Getting list (ID: {}) items...", id);
            var items = await _listService.GetItemsAsync(id);
            _logger.LogInformation("Found {} items in list (ID: {})", items.Count(), id);
            return Ok(items);
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItemAsync(int id, [FromBody] AddItemDTO itemDto)
        {
            _logger.LogInformation("Adding new item to list (ID: {})...", id);
            var newItem = await _listService.AddItemAsync(id, itemDto);

            if (newItem == null)
            {
                _logger.LogWarning("New item (Name: {}) was not added to the list (ID: {})", itemDto.Name, id);
            }
            else
            {
                _logger.LogInformation("New item (ID: {}) created and added to list (ID: {})", newItem.Id, id);
            }

            return newItem == null ? BadRequest() : Ok(newItem);
        }

        [HttpDelete("{listId}/items/{itemId}")]
        public Task RemoveItemAsync(int listId, int itemId)
        {
            _logger.LogInformation("Removing item (ID: {}) from list (ID: {})...", itemId, listId);
            return _listService.RemoveItemAsync(listId, itemId);
        }
    }
}
