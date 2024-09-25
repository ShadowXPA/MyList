using System.ComponentModel.DataAnnotations;

namespace MyList.Server.Models
{
    public class ListItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public UserList? List { get; set; }
    }
}
