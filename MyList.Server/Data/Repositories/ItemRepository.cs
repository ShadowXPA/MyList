using Microsoft.EntityFrameworkCore;
using MyList.Server.Models;

namespace MyList.Server.Data.Repositories
{
    public class ItemRepository(
        ILogger<ItemRepository> logger,
        ApplicationDbContext db
        ) : IItemRepository
    {
        private readonly ILogger<ItemRepository> _logger = logger;
        private readonly ApplicationDbContext _db = db;

        public Task<ListItem?> FindAsync(int id, bool includeList = false)
        {
            var items = _db.Items.AsQueryable();

            if (includeList)
            {
                items = items.Include(i => i.List);
            }

            return items.Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _db.Items.AnyAsync(i => i.Id == id);
        }

        public async Task<ListItem?> UpdateAsync(ListItem item)
        {
            var dbItem = await FindAsync(item.Id, true);

            if (dbItem == null)
            {
                _logger.LogWarning("Item (ID: {}) not found", item.Id);
                return null;
            }

            dbItem.Name = item.Name ?? dbItem.Name;
            dbItem.Description = item.Description ?? dbItem.Description;
            dbItem.Description = dbItem.Description == string.Empty ? null : dbItem.Description;
            dbItem.UpdatedAt = item.UpdatedAt ?? DateTime.UtcNow;
            dbItem.List!.UpdatedAt = DateTime.UtcNow;

            var updated = await _db.SaveChangesAsync();
            return updated > 0 ? dbItem : null;
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
