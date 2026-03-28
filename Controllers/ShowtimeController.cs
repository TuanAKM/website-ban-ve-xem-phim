using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Services;
using System.Threading.Tasks;

namespace MiniCinema.Controllers
{
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
    }
}
