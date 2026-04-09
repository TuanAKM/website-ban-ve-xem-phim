using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MiniCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportData = await _context.Ves
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.Phim)
                .Where(v => v.TrangThaiVe == Models.TrangThaiVe.DaBan || v.TrangThaiVe == Models.TrangThaiVe.DaCheckIn)
                .GroupBy(v => new { v.SuatChieuId, v.SuatChieu.Phim.TenPhim, v.SuatChieu.GioBatDau })
                .Select(g => new ReportViewModel
                {
                    SuatChieuId = g.Key.SuatChieuId,
                    TenPhim = g.Key.TenPhim,
                    GioBatDau = g.Key.GioBatDau,
                    TongSoVe = g.Count(),
                    TongDoanhThu = g.Sum(v => v.GiaVe)
                })
                .OrderByDescending(r => r.GioBatDau)
                .ToListAsync();

            ViewBag.SavedReports = await _context.BaoCaoThongKes.OrderByDescending(b => b.NgayTao).ToListAsync();

            return View(reportData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TongHopDuLieu(System.DateTime startDate, System.DateTime endDate)
        {
            if (startDate > endDate)
            {
                TempData["Error"] = "Ngày bắt đầu không được lớn hơn ngày kết thúc.";
                return RedirectToAction(nameof(Index));
            }

            var query = _context.Ves
                .Include(v => v.SuatChieu)
                .Where(v => (v.TrangThaiVe == Models.TrangThaiVe.DaBan || v.TrangThaiVe == Models.TrangThaiVe.DaCheckIn)
                            && v.SuatChieu != null
                            && v.SuatChieu.GioBatDau >= startDate 
                            && v.SuatChieu.GioBatDau <= endDate);

            int tongSoVe = await query.CountAsync();
            decimal tongDoanhThu = tongSoVe > 0 ? await query.SumAsync(v => v.GiaVe) : 0;

            var baoCao = new Models.BaoCaoThongKe
            {
                TenBaoCao = $"Báo cáo doanh thu từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}",
                NgayBatDau = startDate,
                NgayKetThuc = endDate,
                TongSoVe = tongSoVe,
                TongDoanhThu = tongDoanhThu
            };

            _context.BaoCaoThongKes.Add(baoCao);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã tổng hợp và lưu chốt sổ báo cáo thành công!";
            return RedirectToAction(nameof(Index));
        }
    }

    public class ReportViewModel
    {
        public string SuatChieuId { get; set; } = null!;
        public string TenPhim { get; set; } = null!;
        public System.DateTime GioBatDau { get; set; }
        public int TongSoVe { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}
