using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Controllers
{
    public class MatHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> IndexMH(ProductFilterViewModel filter)
        {
            return await GetProductListAsync(filter);
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductFilterViewModel filter)
        {
            return await GetProductListAsync(filter);
        }

        private async Task<IActionResult> GetProductListAsync(ProductFilterViewModel filter)
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            filter.Categories = categories;

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .Where(p => p.Status == ProductStatus.Approved);

            // 1. Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(p => p.Title.Contains(kw) || p.Description.Contains(kw) || (p.Location != null && p.Location.Contains(kw)));
            }

            // 2. Lọc theo danh mục
            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(filter.CategorySlug) && filter.CategorySlug != "all")
            {
                var selectedCat = categories.FirstOrDefault(c => c.Slug == filter.CategorySlug);
                if (selectedCat != null)
                {
                    query = query.Where(p => p.CategoryId == selectedCat.Id);
                    filter.CategoryId = selectedCat.Id;
                }
            }

            // 3. Lọc theo khoảng giá
            if (!string.IsNullOrWhiteSpace(filter.PriceRange) && filter.PriceRange != "all")
            {
                switch (filter.PriceRange)
                {
                    case "under100":
                        query = query.Where(p => p.Price < 100000);
                        break;
                    case "100to500":
                        query = query.Where(p => p.Price >= 100000 && p.Price <= 500000);
                        break;
                    case "500to1000":
                        query = query.Where(p => p.Price > 500000 && p.Price <= 1000000);
                        break;
                    case "over1000":
                        query = query.Where(p => p.Price > 1000000);
                        break;
                }
            }
            else
            {
                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }
                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }
            }

            // 4. Lọc theo tình trạng
            if (filter.Condition.HasValue)
            {
                query = query.Where(p => p.Condition == filter.Condition.Value);
            }

            // 5. Lọc theo địa điểm
            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(p => p.Location != null && p.Location.Contains(filter.Location.Trim()));
            }

            // 6. Sắp xếp
            query = filter.SortBy switch
            {
                "price-asc" => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // 7. Phân trang
            filter.TotalItems = await query.CountAsync();
            if (filter.Page < 1) filter.Page = 1;
            filter.PageSize = 9;

            filter.Products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return View("IndexMH", filter);
        }
    }
}

