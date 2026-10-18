using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var baseQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Status == ProductStatus.Approved);

            var newestProducts = await baseQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var featuredProducts = await baseQuery
                .OrderBy(p => p.Price)
                .Take(8)
                .ToListAsync();

            var studentBudgetProducts = await baseQuery
                .Where(p => p.Price <= 500000)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var model = new HomeIndexViewModel
            {
                Categories = categories,
                NewestProducts = newestProducts,
                FeaturedProducts = featuredProducts,
                StudentBudgetProducts = studentBudgetProducts
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
