using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniMarket.Models;

namespace UniMarket.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Khởi tạo Roles nếu chưa có
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Đảm bảo các tài khoản mẫu thử nghiệm luôn hoạt động với đúng mật khẩu
            await EnsureUserAsync(userManager, "admin@unimarket.vn", "Admin@123456", "Quản trị viên UniMarket", "Admin", "Đại học Quốc gia TP.HCM, Thủ Đức");
            await EnsureUserAsync(userManager, "nguyen.van.a@example.com", "Student@123456", "Nguyễn Văn A", "User", "Ký túc xá Khu A, ĐHQG TP.HCM");
            await EnsureUserAsync(userManager, "tran.thi.b@example.com", "Student@123456", "Trần Thị B", "User", "Ký túc xá Khu B, ĐHQG TP.HCM");
            await EnsureUserAsync(userManager, "sinhvien1@unimarket.vn", "User@123456", "Sinh viên UniMarket", "User", "TP. Hồ Chí Minh");
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role,
            string location)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = location
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                user.IsActive = true;
                await userManager.UpdateAsync(user);

                var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    await userManager.ResetPasswordAsync(user, resetToken, password);
                }
            }
        }
    }
}

