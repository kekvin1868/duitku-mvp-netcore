using DuitkuMvpNetApp.DTO;

namespace DuitkuMvpNetApp.Models
{
    public class DataItem
    {
        public string Description { get; set; } = string.Empty;
        public List<ItemDTO> Items { get; set; } = new List<ItemDTO>();
    }
}
