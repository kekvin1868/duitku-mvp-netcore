using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DuitkuMvpNetApp.Data;
using DuitkuMvpNetApp.Models;
using DuitkuMvpNetApp.ViewModels;

namespace DuitkuMvpNetApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/Menu")]
        public async Task<IActionResult> Index()
        {
            string merchantOrderId = Request.Query["merchantOrderId"];
            string referenceNumber = Request.Query["reference"];

            var items = await _context.Items
                .Where(i => i.MsItemsTransactionId == merchantOrderId)
                .ToListAsync();
            
            var viewModel = new MenuViewModel
            {
                Items = items
            };

            return View(viewModel);
        }
    }
}