using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Models;
using UniMarket.ViewModels;

namespace UniMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> IndexNguoiDungAdmin(AdminUserListViewModel model)
        {
            return await GetUserListAsync(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(AdminUserListViewModel model)
        {
            return await GetUserListAsync(model);
        }

        private async Task<IActionResult> GetUserListAsync(AdminUserListViewModel model)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                var kw = model.Keyword.Trim();
                query = query.Where(u => u.FullName.Contains(kw) || (u.Email != null && u.Email.Contains(kw)) || (u.PhoneNumber != null && u.PhoneNumber.Contains(kw)));
            }

            model.TotalItems = await query.CountAsync();
            if (model.Page < 1) model.Page = 1;
            model.PageSize = 10;

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            model.Users = users;

            // Lấy danh sách Role của từng User
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.UserRoles[user.Id] = string.Join(", ", roles);
            }

            return View("IndexNguoiDungAdmin", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản quản trị của chính mình.";
                return RedirectToAction(nameof(IndexNguoiDungAdmin));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = user.IsActive 
                    ? $"Đã mở khóa tài khoản {user.Email}." 
                    : $"Đã khóa tài khoản {user.Email}.";
            }

            return RedirectToAction(nameof(IndexNguoiDungAdmin));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Không cho phép xóa tài khoản đang đăng nhập
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Tài khoản không hợp lệ.";
                return RedirectToAction(nameof(IndexNguoiDungAdmin));
            }

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "Bạn không thể xóa tài khoản quản trị đang đăng nhập.";
                return RedirectToAction(nameof(IndexNguoiDungAdmin));
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần xóa.";
                return RedirectToAction(nameof(IndexNguoiDungAdmin));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] =
                    $"Đã xóa tài khoản {user.Email} thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(
                    " ",
                    result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(IndexNguoiDungAdmin));
        }

    }
}

