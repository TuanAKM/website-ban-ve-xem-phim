using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCinema.Controllers
{
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
                .Where(v => v.TrangThaiVe == Models.TrangThaiVe.DaBan)
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

            return View(reportData);
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
