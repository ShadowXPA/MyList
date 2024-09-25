using System.ComponentModel.DataAnnotations;

namespace MyList.Server.DTOs
{
    public class CreateListDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
