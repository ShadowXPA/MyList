using MyList.Server.Models;

namespace MyList.Server.Data.Repositories
{
    public interface IItemRepository : IRepository
    {
        Task<ListItem?> FindAsync(int id, bool includeList = false);
        async Task<bool> ExistsAsync(int id) => (await FindAsync(id)) != null;
        Task<ListItem?> UpdateAsync(ListItem item);
    }
}
