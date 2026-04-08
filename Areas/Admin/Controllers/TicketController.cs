using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.Models;
using System.Threading.Tasks;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchCode = "")
        {
            ViewBag.SearchCode = searchCode;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Scan(string maVe)
        {
            if (string.IsNullOrEmpty(maVe)) return RedirectToAction("Index");

            var ve = await _context.Ves
                .Include(v => v.Ghe)
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.Phim)
                .Include(v => v.SuatChieu.PhongChieu)
                .FirstOrDefaultAsync(v => v.MaVe == maVe);

            if (ve == null)
            {
                TempData["Message"] = "Không tìm thấy vé hợp lệ.";
                TempData["IsComplete"] = false;
                return RedirectToAction("Index");
            }

            if (ve.TrangThaiVe == TrangThaiVe.DaCheckIn)
            {
                TempData["Message"] = "Vé này đã được CHECK-IN rồi!";
                TempData["IsComplete"] = false;
            }
            else if (ve.TrangThaiVe == TrangThaiVe.Huy)
            {
                 TempData["Message"] = "Vé này đã bị HỦY!";
                 TempData["IsComplete"] = false;
            }
            else
            {
                // Soát vé thành công
                ve.TrangThaiVe = TrangThaiVe.DaCheckIn;
                await _context.SaveChangesAsync();
                
                TempData["Message"] = $"Soát vé thành công! Ghế {ve.Ghe?.MaGhe} - Phim {ve.SuatChieu?.Phim?.TenPhim}.";
                TempData["IsComplete"] = true;
            }

            return RedirectToAction("Index", new { searchCode = maVe });
        }

        public async Task<IActionResult> Print(string maVe)
        {
            if (string.IsNullOrEmpty(maVe)) return NotFound();

            var ve = await _context.Ves
                .Include(v => v.Ghe)
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.Phim)
                .Include(v => v.SuatChieu.PhongChieu)
                .FirstOrDefaultAsync(v => v.MaVe == maVe);

            if (ve == null) return NotFound("Chưa xuất hóa đơn cho vé này.");

            return View(ve);
        }
    }
}
