using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.DTOs;
using MiniCinema.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName ?? "N/A",
                    Email = user.Email ?? "N/A",
                    HoTen = user.HoTen ?? "N/A",
                    PhoneNumber = user.PhoneNumber ?? "N/A",
                    Roles = roles,
                    IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > System.DateTimeOffset.UtcNow
                });
            }
            return View(model);
        }

        public IActionResult Create()
        {
            // Admin only creates Staff or other Admin
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    HoTen = dto.HoTen,
                    PhoneNumber = dto.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    // Gán Role (Staff hoặc Admin)
                    if (await _roleManager.RoleExistsAsync(dto.Role))
                    {
                        await _userManager.AddToRoleAsync(user, dto.Role);
                    }
                    return RedirectToAction(nameof(Index));
                }
                foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
            }
            return View(dto);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var dto = new UserEditDto
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                HoTen = user.HoTen ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditDto dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                user.HoTen = dto.HoTen;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(dto.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                    }
                    return RedirectToAction(nameof(Index));
                }
                foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                TempData["Error"] = "Bạn không thể tự xóa chính tài khoản Admin của mình!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Cấm xóa tài khoản đồng cấp Admin!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    TempData["Error"] = "Lỗi khi xóa tài khoản: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
                else
                {
                    TempData["Success"] = "Đã xóa tài khoản " + user.UserName;
                }
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Bảo vệ hệ thống: Khách hàng này đã mua vé! Vui lòng dùng tính năng [Khóa Tài Khoản] thay vì xóa để bảo toàn báo cáo doanh thu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Cấm khóa tài khoản đồng cấp Admin!";
                return RedirectToAction(nameof(Index));
            }

            bool isCurrentlyLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > System.DateTimeOffset.UtcNow;
            
            if (isCurrentlyLocked)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Đã MỞ KHÓA tài khoản {user.UserName}.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, System.DateTimeOffset.MaxValue);
                TempData["Success"] = $"Đã KHÓA tài khoản {user.UserName} vĩnh viễn.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
