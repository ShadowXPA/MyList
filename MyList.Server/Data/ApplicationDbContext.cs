using Microsoft.EntityFrameworkCore;
using MyList.Server.Models;

namespace MyList.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserList> Lists { get; set; }
        public DbSet<ListItem> Items { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
