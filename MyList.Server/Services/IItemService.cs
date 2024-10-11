using MyList.Server.DTOs;

namespace MyList.Server.Services
{
    public interface IItemService
    {
        Task<ItemDTO?> EditItemAsync(int id, EditItemDTO itemDto);
        Task<ItemDTO?> MoveItemAsync(int id, int listId);
    }
}
