using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Controllers
{
    public class CTMatHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CTMatHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> IndexCT(int? id)
        {
            if (id == null)
            {
                return NotFound("Mã sản phẩm không hợp lệ.");
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Owner)
                .Include(p => p.Images.OrderBy(img => img.DisplayOrder))
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm yêu cầu.");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = !string.IsNullOrEmpty(currentUserId) && product.OwnerId == currentUserId;
            var isAdmin = User.IsInRole("Admin");

            // Chỉ hiển thị sản phẩm chưa được duyệt cho chính người bán hoặc Admin
            if (product.Status != ProductStatus.Approved && !isOwner && !isAdmin)
            {
                return NotFound("Sản phẩm chưa được công khai hoặc đang chờ duyệt.");
            }

            // Kiểm tra trạng thái yêu thích
            bool isFavorite = false;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                isFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == currentUserId && f.ProductId == product.Id);
            }

            // Sản phẩm liên quan cùng danh mục
            var relatedProducts = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.Status == ProductStatus.Approved)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                IsFavorite = isFavorite,
                IsOwner = isOwner
            };

            return View(viewModel);
        }
    }
}

