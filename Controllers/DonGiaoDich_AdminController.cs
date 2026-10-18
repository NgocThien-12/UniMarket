using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DonGiaoDich_AdminController : Controller
    {
        public IActionResult IndexGiaoDichAdmin()
        {
            return RedirectToAction("IndexGiaoDichAdmin", "Order", new { area = "Admin" });
        }
    }
}

