using MyList.Server.Models;

namespace MyList.Server.Data.Repositories
{
    public interface IListRepository : IRepository
    {
        Task<IEnumerable<UserList>> GetAllAsync(IList<int>? ids = null);
        Task<UserList?> FindAsync(int id, IList<int>? ids = null, bool includeItems = false);
        async Task<bool> ExistsAsync(int id) => (await FindAsync(id)) != null;
        Task<UserList?> InsertAsync(UserList list);
        Task<UserList?> UpdateAsync(UserList list);
        Task<bool> DeleteAsync(int id);
        Task<ListItem?> AddItemAsync(int listId, ListItem item);
        Task<bool> RemoveItemAsync(int listId, int itemId);
    }
}
