using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MiniCinema.Models;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShowtimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(ApplicationDbContext context, IShowtimeService showtimeService)
        {
            _context = context;
            _showtimeService = showtimeService;
        }

        public async Task<IActionResult> Index()
        {
            var showtimes = await _context.SuatChieus.Include(s => s.Phim).Include(s => s.PhongChieu).ToListAsync();
            return View(showtimes);
        }

        public IActionResult Create()
        {
            ViewBag.PhimId = new SelectList(_context.Phims, "MaPhim", "TenPhim");
            ViewBag.PhongChieuId = new SelectList(_context.PhongChieus, "MaPhong", "TenPhong");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShowtimeCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _showtimeService.CreateShowtimeAsync(dto);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Error ?? "Lỗi không xác định");
            }

            ViewBag.PhimId = new SelectList(_context.Phims, "MaPhim", "TenPhim", dto.PhimId);
            ViewBag.PhongChieuId = new SelectList(_context.PhongChieus, "MaPhong", "TenPhong", dto.PhongChieuId);
            return View(dto);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var sc = await _context.SuatChieus
                .Include(s => s.Phim)
                .Include(s => s.PhongChieu)
                .FirstOrDefaultAsync(m => m.MaSuatChieu == id);
            if (sc == null) return NotFound();
            return View(sc);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var sc = await _context.SuatChieus.FindAsync(id);
            if (sc == null) return NotFound();

            var hasTickets = await _context.Ves.AnyAsync(v => v.SuatChieuId == id && v.TrangThaiVe != TrangThaiVe.Huy);
            ViewBag.HasTickets = hasTickets;

            ViewBag.PhimId = new SelectList(_context.Phims, "MaPhim", "TenPhim", sc.PhimId);
            ViewBag.PhongChieuId = new SelectList(_context.PhongChieus, "MaPhong", "TenPhong", sc.PhongChieuId);
            return View(sc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SuatChieu sc)
        {
            if (id != sc.MaSuatChieu) return NotFound();

            var hasTickets = await _context.Ves.AnyAsync(v => v.SuatChieuId == id && v.TrangThaiVe != TrangThaiVe.Huy);
            if (hasTickets)
            {
                ModelState.AddModelError("", "Suất chiếu này đã có khách đặt vé, không thể sửa lịch hoặc phòng.");
                ViewBag.HasTickets = true;
                ViewBag.PhimId = new SelectList(_context.Phims, "MaPhim", "TenPhim", sc.PhimId);
                ViewBag.PhongChieuId = new SelectList(_context.PhongChieus, "MaPhong", "TenPhong", sc.PhongChieuId);
                return View(sc);
            }

            if (ModelState.IsValid)
            {
                var original = await _context.SuatChieus.Include(s => s.Phim).FirstOrDefaultAsync(s => s.MaSuatChieu == id);
                if (original != null)
                {
                    original.GioBatDau = sc.GioBatDau;
                    original.GioKetThuc = sc.GioBatDau.AddMinutes(original.Phim?.ThoiLuong ?? 120);
                    original.PhimId = sc.PhimId;
                    original.PhongChieuId = sc.PhongChieuId;

                    // TODO: Recheck Overlap using ShowtimeService logic ideally. But for simplicity:
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PhimId = new SelectList(_context.Phims, "MaPhim", "TenPhim", sc.PhimId);
            ViewBag.PhongChieuId = new SelectList(_context.PhongChieus, "MaPhong", "TenPhong", sc.PhongChieuId);
            return View(sc);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var sc = await _context.SuatChieus
                .Include(s => s.Phim)
                .Include(s => s.PhongChieu)
                .FirstOrDefaultAsync(m => m.MaSuatChieu == id);
            if (sc == null) return NotFound();

            var hasTickets = await _context.Ves.AnyAsync(v => v.SuatChieuId == id && v.TrangThaiVe == TrangThaiVe.DaBan);
            ViewBag.HasTickets = hasTickets;

            return View(sc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sc = await _context.SuatChieus.FindAsync(id);
            if (sc != null)
            {
                var ticketsInfo = _context.Ves.Where(v => v.SuatChieuId == id);
                _context.Ves.RemoveRange(ticketsInfo);
                _context.SuatChieus.Remove(sc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
