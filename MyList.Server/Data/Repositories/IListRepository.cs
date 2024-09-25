using MyList.Server.Models;

namespace MyList.Server.Data.Repositories
{
    public interface IListRepository
    {
        Task<IEnumerable<UserList>> GetAllAsync();
        Task<UserList?> FindAsync(int id, bool includeItems = false);
        async Task<bool> ExistsAsync(int id) => (await FindAsync(id)) != null;
        Task<UserList?> InsertAsync(UserList list);
        Task<bool> UpdateAsync(UserList list);
        Task<bool> DeleteAsync(int id);
        Task<ListItem?> AddItemAsync(int listId, ListItem item);
        Task<bool> RemoveItemAsync(int listId, int itemId);
    }
}
