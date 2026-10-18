using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult IndexAdmin()
        {
            return RedirectToAction("IndexAdmin", "Dashboard", new { area = "Admin" });
        }
    }
}

