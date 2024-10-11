using MyList.Server.Data.Repositories;
using MyList.Server.DTOs;
using MyList.Server.Extensions;
using MyList.Server.Models;

namespace MyList.Server.Services
{
    public class ItemService(
        ILogger<ItemService> logger,
        IItemRepository itemRepository,
        IListRepository listRepository
        ) : IItemService
    {
        private readonly ILogger<ItemService> _logger = logger;
        private readonly IItemRepository _itemRepository = itemRepository;
        private readonly IListRepository _listRepository = listRepository;

        public async Task<ItemDTO?> EditItemAsync(int id, EditItemDTO itemDto)
        {
            var itemExists = await _itemRepository.ExistsAsync(id);

            if (!itemExists)
            {
                _logger.LogWarning("Item (ID: {}) was not found", id);
                return null;
            }

            var item = new ListItem()
            {
                Id = id,
                UpdatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(itemDto.Name))
            {
                item.Name = itemDto.Name;
            }

            if (itemDto.Description != null)
            {
                item.Description = itemDto.Description;
            }

            var updated = await _itemRepository.UpdateAsync(item);

            _logger.LogInformation("Item (ID: {}) was {}", id, (updated != null ? "updated" : "not updated"));

            return updated?.ToDTO();
        }

        public async Task<ItemDTO?> MoveItemAsync(int id, int listId)
        {
            var item = await _itemRepository.FindAsync(id, true);

            if (item == null)
            {
                _logger.LogWarning("Item (ID: {}) was not found", id);
                return null;
            }

            if (item.List!.Id == listId)
            {
                _logger.LogWarning("Item (ID: {}) is already in this list (ID: {})", id, listId);
                return null;
            }

            var newList = await _listRepository.FindAsync(listId);

            if (newList == null)
            {
                _logger.LogWarning("Item (ID: {}) was not moved, new list does not exist", id);
                return null;
            }

            _logger.LogInformation("Moving item (ID: {}) from list (ID: {}) to list (ID: {})...", id, item.List.Id, listId);

            item.List.Items.Remove(item);
            newList.Items.Add(item);
            item.List.UpdatedAt = DateTime.UtcNow;
            newList.UpdatedAt = DateTime.UtcNow;
            item.List = newList;
            item.UpdatedAt = DateTime.UtcNow;

            var moved = (await _itemRepository.SaveChangesAsync()) > 0;

            return moved ? item.ToDTO() : null;
        }
    }
}
