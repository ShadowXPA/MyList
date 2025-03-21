﻿using MyList.Server.Data.Repositories;
using MyList.Server.DTOs;
using MyList.Server.Extensions;
using MyList.Server.Models;

namespace MyList.Server.Services
{
    public class ListService(
        ILogger<ListService> logger,
        IListRepository listRepository,
        IHttpClientFactory httpClientFactory
        ) : IListService
    {
        private readonly ILogger<ListService> _logger = logger;
        private readonly IListRepository _listRepository = listRepository;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<ListDTO>> GetListsAsync(string? query)
        {
            if (query != null)
            {
                var httpClient = _httpClientFactory.CreateClient("SearchEngine");
                var ids = await httpClient.GetFromJsonAsync<IList<int>>($"/api/search/lists?q=${Uri.EscapeDataString(query)}");
                return (await _listRepository.GetAllAsync(ids)).ToDTO();
            }

            return (await _listRepository.GetAllAsync()).ToDTO();
        }

        public async Task<ListDTO?> GetListAsync(int id, string? query, bool includeItems = false)
        {
            IList<int>? ids = null;

            if (includeItems && query != null)
            {
                var httpClient = _httpClientFactory.CreateClient("SearchEngine");
                ids = await httpClient.GetFromJsonAsync<IList<int>>($"/api/search/items?q=${Uri.EscapeDataString(query)}");
            }

            return (await _listRepository.FindAsync(id, ids, includeItems))?.ToDTO();
        }

        public async Task<ListDTO?> CreateListAsync(CreateListDTO listDto)
        {
            if (string.IsNullOrWhiteSpace(listDto.Name))
            {
                _logger.LogWarning("Can not create new list... Name is null or white space");
                return null;
            }

            var list = new UserList()
            {
                Name = listDto.Name,
                Description = listDto.Description
            };

            return (await _listRepository.InsertAsync(list))?.ToDTO();
        }

        public async Task<ListDTO?> EditListAsync(int id, EditListDTO listDto)
        {
            var listExists = await _listRepository.ExistsAsync(id);

            if (!listExists)
            {
                _logger.LogWarning("List (ID: {}) was not found", id);
                return null;
            }

            var list = new UserList()
            {
                Id = id,
                UpdatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(listDto.Name))
            {
                list.Name = listDto.Name;
            }

            if (listDto.Description != null)
            {
                list.Description = listDto.Description;
            }

            var updated = await _listRepository.UpdateAsync(list);

            _logger.LogInformation("List (ID: {}) was {}", id, (updated != null ? "updated" : "not updated"));

            return updated?.ToDTO();
        }

        public async Task DeleteListAsync(int id)
        {
            var deleted = await _listRepository.DeleteAsync(id);
            _logger.LogInformation("List (ID: {}) was {}", id, (deleted ? "deleted" : "not deleted"));
        }

        public async Task<IEnumerable<ItemDTO>> GetItemsAsync(int listId, string? query)
        {
            IList<int>? ids = null;

            if (query != null)
            {
                var httpClient = _httpClientFactory.CreateClient("SearchEngine");
                ids = await httpClient.GetFromJsonAsync<IList<int>>($"/api/search/items?q=${Uri.EscapeDataString(query)}");
            }

            var list = await _listRepository.FindAsync(listId, ids, true);

            if (list == null)
            {
                _logger.LogWarning("List (ID: {}) was not found", listId);
                return new List<ItemDTO>();
            }

            return list.Items.ToDTO();
        }

        public async Task<ItemDTO?> AddItemAsync(int listId, AddItemDTO itemDto)
        {
            var listExists = await _listRepository.ExistsAsync(listId);

            if (!listExists)
            {
                _logger.LogWarning("List (ID: {}) was not found", listId);
                return null;
            }

            if (string.IsNullOrEmpty(itemDto.Name))
            {
                _logger.LogWarning("Can not add new item... Name is null or white space");
                return null;
            }

            var item = new ListItem()
            {
                Name = itemDto.Name,
                Description = itemDto.Description
            };

            return (await _listRepository.AddItemAsync(listId, item))?.ToDTO();
        }

        public async Task RemoveItemAsync(int listId, int itemId)
        {
            var removed = await _listRepository.RemoveItemAsync(listId, itemId);
            _logger.LogInformation("Item (ID: {}) was {} from list (ID: {})", itemId, (removed ? "removed" : "not removed"), listId);
        }
    }
}
