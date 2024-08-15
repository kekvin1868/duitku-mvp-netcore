using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DuitkuMvpNetApp.Data;
using DuitkuMvpNetApp.Models;
using DuitkuMvpNetApp.ControllerModels;

namespace DuitkuMvpNetApp.Factories.Transactions
{
    public class TransactionFactory
    {
        private readonly ApplicationDbContext _context;

        public TransactionFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateNewTransactionAsync (CreateTransactionPayloadModel payload)
        {
            return new Transaction
            {
                MsTransactionId = payload.TransactionId,
                MsTransactionTotalAmount = payload.TotalAmount,
                MsTransactionTimestamp = DateTime.UtcNow,
                MsTransactionDescription = payload.Description,
                MsTransactionStatus = TransactionStatus.Pending
            };
        }

        internal string GenerateUniqueTransactionId()
        {
            string uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            return $"SS-{uniquePart}";
        }
    }
}