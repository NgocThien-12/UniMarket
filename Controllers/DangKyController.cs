using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    public class DangKyController : Controller
    {
        [HttpGet]
        public IActionResult IndexDK(string? returnUrl = null)
        {
            return RedirectToAction("Register", "Account", new { returnUrl });
        }
    }
}

