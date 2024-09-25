using System.ComponentModel.DataAnnotations;

namespace MyList.Server.Models
{
    public class UserList
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public IList<ListItem>? Items { get; set; }
    }
}
