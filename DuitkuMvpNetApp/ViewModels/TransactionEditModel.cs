using DuitkuMvpNetApp.Models;

namespace DuitkuMvpNetApp.ViewModels
{
    public class TransactionEditModel
    {
        public Transaction TransactionDetails { get; set; }
        public List<Item> ItemDetails { get; set; } = new List<Item>();
    }
}