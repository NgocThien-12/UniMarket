using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> IndexSpAdmin(AdminProductListViewModel model)
        {
            return await GetProductListAsync(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(AdminProductListViewModel model)
        {
            return await GetProductListAsync(model);
        }

        private async Task<IActionResult> GetProductListAsync(AdminProductListViewModel model)
        {
            var categories = await _context.Categories.ToListAsync();
            model.Categories = categories;

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                var kw = model.Keyword.Trim();
                query = query.Where(p => p.Title.Contains(kw) || (p.Owner != null && p.Owner.FullName.Contains(kw)));
            }

            if (model.Status.HasValue)
            {
                query = query.Where(p => p.Status == model.Status.Value);
            }

            if (model.CategoryId.HasValue && model.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == model.CategoryId.Value);
            }

            model.TotalItems = await query.CountAsync();
            if (model.Page < 1) model.Page = 1;
            model.PageSize = 10;

            model.Products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            return View("IndexSpAdmin", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = ProductStatus.Approved;
                product.RejectionReason = null;
                product.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã duyệt sản phẩm #{product.Id}: \"{product.Title}\"";
            }
            return RedirectToAction(nameof(IndexSpAdmin));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = ProductStatus.Rejected;
                product.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "Nội dung vi phạm quy định đăng tin." : reason.Trim();
                product.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã từ chối tin đăng #{product.Id}.";
            }
            return RedirectToAction(nameof(IndexSpAdmin));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = ProductStatus.Inactive;
                product.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã vô hiệu hóa sản phẩm #{product.Id}.";
            }
            return RedirectToAction(nameof(IndexSpAdmin));
        }
    }
}

