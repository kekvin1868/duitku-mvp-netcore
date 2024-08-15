using System.Collections.Generic;
using DuitkuMvpNetApp.Models;

namespace DuitkuMvpNetApp.ViewModels
{
    public class TransactionViewModel
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}