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
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> IndexGiaoDichAdmin(AdminOrderListViewModel model)
        {
            return await GetOrderListAsync(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(AdminOrderListViewModel model)
        {
            return await GetOrderListAsync(model);
        }

        private async Task<IActionResult> GetOrderListAsync(AdminOrderListViewModel model)
        {
            var query = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                var kw = model.Keyword.Trim();
                query = query.Where(o => o.OrderCode.Contains(kw) || o.FullName.Contains(kw) || o.PhoneNumber.Contains(kw));
            }

            if (model.Status.HasValue)
            {
                query = query.Where(o => o.Status == model.Status.Value);
            }

            model.TotalItems = await query.CountAsync();
            if (model.Page < 1) model.Page = 1;
            model.PageSize = 10;

            model.Orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            return View("IndexGiaoDichAdmin", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Nếu hủy đơn hàng, hoàn trả trạng thái sản phẩm
            if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    if (item.Product != null && item.Product.Status == ProductStatus.Sold)
                    {
                        item.Product.Status = ProductStatus.Approved;
                        item.Product.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }
                order.CancelledAt = DateTimeOffset.UtcNow;
                order.CancelledBy = "Admin";
                order.CancellationReason = "Quản trị viên cập nhật hủy đơn";
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng {order.OrderNumber} thành công.";

            return RedirectToAction(nameof(IndexGiaoDichAdmin));
        }
    }
}
