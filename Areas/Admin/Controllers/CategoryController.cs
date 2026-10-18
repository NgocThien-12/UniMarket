using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;

namespace UniMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                TempData["ErrorMessage"] = "Tên danh mục không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-").Replace("&", "");
            }

            if (Request.Form.ContainsKey("Icon") && !string.IsNullOrWhiteSpace(Request.Form["Icon"]))
            {
                category.IconClass = Request.Form["Icon"].ToString().Trim();
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã thêm danh mục mới thành công.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            if (category.Products.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa danh mục \"{category.Name}\" vì vẫn còn {category.Products.Count} sản phẩm liên kết.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa danh mục thành công.";

            return RedirectToAction(nameof(Index));
        }
    }
}

