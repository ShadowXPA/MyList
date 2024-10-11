﻿using Microsoft.EntityFrameworkCore;
using MyList.Server.Models;

namespace MyList.Server.Data.Repositories
{
    public class ListRepository(
        ILogger<ListRepository> logger,
        ApplicationDbContext db
        ) : IListRepository
    {
        private readonly ILogger<ListRepository> _logger = logger;
        private readonly ApplicationDbContext _db = db;

        public Task<IEnumerable<UserList>> GetAllAsync(string? query)
        {
            var lists = _db.Lists
                .OrderByDescending(l => l.UpdatedAt == null ? l.CreatedAt : l.UpdatedAt)
                .AsQueryable();

            if (query != null)
            {
                lists = lists.Where(l =>
                     l.Name!.ToUpper().Contains(query.ToUpper()) || (l.Description != null ? l.Description.ToUpper().Contains(query.ToUpper()) : false)
                );
            }

            return Task.FromResult(lists.AsEnumerable());
        }

        public Task<UserList?> FindAsync(int id, string? query = null, bool includeItems = false)
        {
            var lists = _db.Lists.AsQueryable();

            if (includeItems)
            {
                lists = lists.Include(l => l.Items
                    .OrderByDescending(i => i.UpdatedAt == null ? i.CreatedAt : i.UpdatedAt)
                    .Where(i =>
                        query == null ||
                        i.Name!.ToUpper().Contains(query.ToUpper()) ||
                        (i.Description != null ? i.Description.ToUpper().Contains(query.ToUpper()) : false))
                    );
            }

            return lists.Where(l => l.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _db.Lists.AnyAsync(l => l.Id == id);
        }

        public async Task<UserList?> InsertAsync(UserList list)
        {
            var newList = await _db.Lists.AddAsync(list);
            var created = await _db.SaveChangesAsync();
            return created > 0 ? newList.Entity : null;
        }

        public async Task<UserList?> UpdateAsync(UserList list)
        {
            var dbList = await FindAsync(list.Id, includeItems: true);

            if (dbList == null)
            {
                _logger.LogWarning("List (ID: {}) not found", list.Id);
                return null;
            }

            dbList.Name = list.Name ?? dbList.Name;
            dbList.Description = list.Description ?? dbList.Description;
            dbList.Description = dbList.Description == string.Empty ? null : dbList.Description;
            dbList.UpdatedAt = list.UpdatedAt ?? DateTime.UtcNow;

            var updated = await _db.SaveChangesAsync();
            return updated > 0 ? dbList : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var list = await _db.Lists.FindAsync(id);

            if (list == null)
            {
                _logger.LogWarning("List (ID: {}) not found", id);
                return false;
            }

            _db.Lists.Remove(list);

            var removed = await _db.SaveChangesAsync();
            return removed > 0;
        }

        public async Task<ListItem?> AddItemAsync(int listId, ListItem item)
        {
            var list = await _db.Lists
                .Include(l => l.Items)
                .Where(l => l.Id == listId)
                .FirstOrDefaultAsync();

            if (list == null)
            {
                _logger.LogWarning("List (ID: {}) not found", listId);
                return null;
            }

            list.UpdatedAt = DateTime.UtcNow;
            list.Items.Add(item);
            item.List = list;

            var created = await _db.SaveChangesAsync();
            return created > 0 ? item : null;
        }

        public async Task<bool> RemoveItemAsync(int listId, int itemId)
        {
            var list = await _db.Lists
                .Include(l => l.Items)
                .Where(l => l.Id == listId)
                .FirstOrDefaultAsync();

            if (list == null)
            {
                _logger.LogWarning("List (ID: {}) not found", listId);
                return false;
            }

            var item = list.Items.Where(i => i.Id == itemId).FirstOrDefault();

            if (item == null)
            {
                _logger.LogWarning("Item (ID: {}) not found on list (ID: {})", itemId, listId);
                return false;
            }

            list.UpdatedAt = DateTime.UtcNow;
            list.Items.Remove(item);
            item.List = null;

            var removed = await _db.SaveChangesAsync();
            return removed > 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
