using MyList.Server.DTOs;

namespace MyList.Server.Services
{
    public interface IListService
    {
        Task<IEnumerable<ListDTO>> GetListsAsync(string? query);
        Task<ListDTO?> GetListAsync(int id, string? query, bool includeItems = false);
        Task<ListDTO?> CreateListAsync(CreateListDTO listDto);
        Task<ListDTO?> EditListAsync(int id, EditListDTO listDto);
        Task DeleteListAsync(int id);
        Task<IEnumerable<ItemDTO>> GetItemsAsync(int listId, string? query);
        Task<ItemDTO?> AddItemAsync(int listId, AddItemDTO itemDto);
        Task RemoveItemAsync(int listId, int itemId);
    }
}
