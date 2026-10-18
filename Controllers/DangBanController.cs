using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Controllers
{
    [Authorize]
    public class DangBanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DangBanController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> IndexDB()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var model = new ProductCreateViewModel
            {
                Categories = categories,
                AvailableQuantity = 1,
                Condition = ProductCondition.Good
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexDB(ProductCreateViewModel model)
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            model.Categories = categories;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var product = new Product
            {
                Title = model.Name.Trim(),
                Description = model.Description.Trim(),
                Price = model.Price,
                Condition = model.Condition,
                Location = model.Location.Trim(),
                CategoryId = model.CategoryId,
                OwnerId = userId,
                Status = ProductStatus.Approved, // Hiển thị trên sàn
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Xử lý upload ảnh
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            bool hasMainImage = false;

            if (model.Images != null && model.Images.Count > 0)
            {
                int sort = 1;
                foreach (var file in model.Images.Take(5))
                {
                    if (file.Length > 0 && file.Length <= 5 * 1024 * 1024)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (allowedExtensions.Contains(ext))
                        {
                            var uniqueFileName = $"prod_{product.Id}_{Guid.NewGuid():N}{ext}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = $"/uploads/products/{uniqueFileName}",
                                IsPrimary = !hasMainImage,
                                DisplayOrder = sort++,
                                CreatedAt = DateTimeOffset.UtcNow
                            };
                            _context.ProductImages.Add(productImage);
                            hasMainImage = true;
                        }
                    }
                }
            }

            if (!hasMainImage)
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f?w=800&h=600&fit=crop&auto=format",
                    IsPrimary = true,
                    DisplayOrder = 1,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng tin thành công! Sản phẩm của bạn đã xuất hiện trên UniMarket.";
            return RedirectToAction("MyListings", "Account");
        }
    }
}

