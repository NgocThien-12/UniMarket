using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SPAdminController : Controller
    {
        public IActionResult IndexSpAdmin()
        {
            return RedirectToAction("IndexSpAdmin", "Product", new { area = "Admin" });
        }
    }
}

