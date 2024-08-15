using DuitkuMvpNetApp.Models;
using DuitkuMvpNetApp.DTO;

namespace DuitkuMvpNetApp.ControllerModels
{
    public class TransactionControllerModel
    {
        public string TransactionId { get; set; }
        public double TotalAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<ItemDTO> Items {get; set; } = new List<ItemDTO>();

    }
}