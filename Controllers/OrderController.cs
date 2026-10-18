using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Data;
using UniMarket.Models;
using UniMarket.Services;
using UniMarket.ViewModels;

namespace UniMarket.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            ApplicationDbContext context,
            ICartService cartService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _cartService = cartService;
            _userManager = userManager;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartService.GetCartAsync(CurrentUserId);
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng chọn sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                RecipientName = user?.FullName ?? "",
                PhoneNumber = user?.PhoneNumber ?? "",
                DeliveryAddress = user?.Address ?? "",
                Cart = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = await _cartService.GetCartAsync(CurrentUserId);
            model.Cart = cart;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _cartService.CheckoutAsync(CurrentUserId, model);
            if (!result.Success || result.Order == null)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Confirmation), new { id = result.Order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == CurrentUserId);

            if (order == null)
            {
                return NotFound("Không tìm thấy thông tin đơn hàng.");
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Bảo mật: Người dùng chỉ được xem đơn hàng của chính mình (trừ Admin)
            if (order.BuyerId != CurrentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = new OrderDetailViewModel
            {
                Order = order
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == CurrentUserId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng khi đơn hàng đang ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Cập nhật lại trạng thái sản phẩm
            foreach (var item in order.Items)
            {
                if (item.Product != null && item.Product.Status == ProductStatus.Sold)
                {
                    item.Product.Status = ProductStatus.Approved;
                    item.Product.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTimeOffset.UtcNow;
            order.CancelledBy = CurrentUserId;
            order.CancellationReason = "Người mua hủy đơn hàng";
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
