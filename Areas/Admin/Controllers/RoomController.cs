using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PhongChieus.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            if (await _context.PhongChieus.AnyAsync(p => p.MaPhong == dto.MaPhong))
            {
                ModelState.AddModelError("", "Mã phòng đã tồn tại.");
                return View(dto);
            }

            var tongGhe = dto.SoHang * dto.SoGheMoiHang;
            var phong = new PhongChieu
            {
                MaPhong = dto.MaPhong,
                TenPhong = dto.TenPhong,
                LoaiPhong = dto.LoaiPhong,
                TongSoGhe = tongGhe
            };

            var listGhe = new List<Ghe>();
            char[] rowLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            for (int r = 0; r < dto.SoHang; r++)
            {
                string rowLabel = rowLetters[r].ToString();
                
                for (int c = 1; c <= dto.SoGheMoiHang; c++)
                {
                    string maGhe = $"{dto.MaPhong}-{rowLabel}{c:D2}";
                    
                    // Giả định: Hàng cuối là ghế Đôi, 2 hàng trước là VIP, còn lại là Thường
                    LoaiGhe loai = LoaiGhe.Don;
                    if (r == dto.SoHang - 1) loai = LoaiGhe.Doi;
                    else if (r >= dto.SoHang - 3) loai = LoaiGhe.Vip;

                    listGhe.Add(new Ghe
                    {
                        MaGhe = maGhe,
                        LoaiGhe = loai,
                        TrangThai = TrangThaiGhe.Trong,
                        PhongChieuId = phong.MaPhong
                    });
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.PhongChieus.AddAsync(phong);
                await _context.Ghes.AddRangeAsync(listGhe);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(dto);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var p = await _context.PhongChieus.FirstOrDefaultAsync(m => m.MaPhong == id);
            if (p == null) return NotFound();
            return View(p);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var p = await _context.PhongChieus.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PhongChieu p)
        {
            if (id != p.MaPhong) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.PhongChieus.FindAsync(id);
                if (existing != null)
                {
                    existing.TenPhong = p.TenPhong;
                    existing.LoaiPhong = p.LoaiPhong;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var p = await _context.PhongChieus.FirstOrDefaultAsync(m => m.MaPhong == id);
            if (p == null) return NotFound();

            var hasShowtime = await _context.SuatChieus.AnyAsync(s => s.PhongChieuId == id);
            ViewBag.HasShowtime = hasShowtime;

            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var p = await _context.PhongChieus.Include(r => r.Ghes).FirstOrDefaultAsync(r => r.MaPhong == id);
            if (p != null)
            {
                // Xoá toàn bộ ghế của phòng (Cascade bằng tay nếu DB không set cascade xoá mềm)
                _context.Ghes.RemoveRange(p.Ghes);
                _context.PhongChieus.Remove(p);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
