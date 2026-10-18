using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    public class DangNhapController : Controller
    {
        [HttpGet]
        public IActionResult IndexLogin(string? returnUrl = null)
        {
            return RedirectToAction("Login", "Account", new { returnUrl });
        }
    }
}

