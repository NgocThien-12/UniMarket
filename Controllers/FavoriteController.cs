using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;

namespace UniMarket.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == CurrentUserId && f.ProductId == productId);

            bool isFavorite;
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                isFavorite = false;
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = CurrentUserId,
                    ProductId = productId,
                    CreatedAt = DateTimeOffset.UtcNow
                });
                isFavorite = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isFavorite, message = isFavorite ? "Đã lưu vào danh sách yêu thích!" : "Đã bỏ yêu thích!" });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var favorites = await _context.Favorites
                .Include(f => f.Product)
                .ThenInclude(p => p!.Category)
                .Include(f => f.Product)
                .ThenInclude(p => p!.Images)
                .Where(f => f.UserId == CurrentUserId && f.Product != null)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(favorites);
        }
    }
}

