namespace MyList.Server.DTOs
{
    public class ListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IList<ItemDTO> Items { get; set; } = new List<ItemDTO>();
    }
}
