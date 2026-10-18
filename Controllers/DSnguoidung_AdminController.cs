using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DSnguoidung_AdminController : Controller
    {
        public IActionResult IndexNguoiDungAdmin()
        {
            return RedirectToAction("IndexNguoiDungAdmin", "User", new { area = "Admin" });
        }
    }
}

