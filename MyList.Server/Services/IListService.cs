using MyList.Server.DTOs;

namespace MyList.Server.Services
{
    public interface IListService
    {
        Task<IEnumerable<ListDTO>> GetListsAsync();
        Task<ListDTO?> GetListAsync(int id, bool includeItems = false);
        Task<ListDTO?> CreateListAsync(CreateListDTO listDto);
        Task EditListAsync(int id, EditListDTO listDto);
        Task DeleteListAsync(int id);
        Task<IEnumerable<ItemDTO>> GetItemsAsync(int listId);
        Task<ItemDTO?> AddItemAsync(int listId, AddItemDTO itemDto);
        Task RemoveItemAsync(int listId, int itemId);
    }
}
