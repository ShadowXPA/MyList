using System.ComponentModel.DataAnnotations;

namespace MyList.Server.Models
{
    public class ListItem
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required UserList List { get; set; }
    }
}
