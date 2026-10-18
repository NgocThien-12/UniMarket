using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniMarket.Services;

namespace UniMarket.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync(CurrentUserId);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
        {
            var result = await _cartService.AddToCartAsync(CurrentUserId, productId, quantity);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var count = await _cartService.GetCartItemCountAsync(CurrentUserId);
                return Json(new { success = result.Success, message = result.Message, cartCount = count });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var result = await _cartService.UpdateQuantityAsync(CurrentUserId, productId, quantity);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = await _cartService.GetCartAsync(CurrentUserId);
                return Json(new { success = result.Success, message = result.Message, totalAmount = cart.TotalAmount, totalQuantity = cart.TotalQuantity });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            await _cartService.RemoveFromCartAsync(CurrentUserId, productId);
            TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(CurrentUserId);
            TempData["SuccessMessage"] = "Đã làm trống giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var count = await _cartService.GetCartItemCountAsync(CurrentUserId);
            return Json(new { count });
        }
    }
}

