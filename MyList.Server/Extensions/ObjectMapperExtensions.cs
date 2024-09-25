using MyList.Server.DTOs;
using MyList.Server.Models;

namespace MyList.Server.Extensions
{
    public static class ObjectMapperExtensions
    {
        public static ListDTO ToDTO(this UserList list)
        {
            return new ListDTO()
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                CreatedAt = list.CreatedAt,
                UpdatedAt = list.UpdatedAt,
                Items = list.Items.ToDTO().ToList()
            };
        }

        public static ItemDTO ToDTO(this ListItem item)
        {
            return new ItemDTO()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                CreatedAt = item.CreatedAt
            };
        }

        public static IEnumerable<ListDTO> ToDTO(this IEnumerable<UserList> lists)
        {
            foreach (var list in lists)
            {
                yield return list.ToDTO();
            }
        }

        public static IEnumerable<ItemDTO> ToDTO(this IEnumerable<ListItem> items)
        {
            foreach (var item in items)
            {
                yield return item.ToDTO();
            }
        }
    }
}
