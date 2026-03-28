using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCinema.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IShowtimeService _showtimeService;
        private readonly IBookingService _bookingService;

        public BookingController(ApplicationDbContext context, IShowtimeService showtimeService, IBookingService bookingService)
        {
            _context = context;
            _showtimeService = showtimeService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var phims = await _context.Phims.ToListAsync();
            return View(phims);
        }

        public async Task<IActionResult> Showtimes(string movieId)
        {
            var showtimes = await _showtimeService.GetAvailableShowtimesAsync(movieId);
            return View(showtimes);
        }

        public async Task<IActionResult> SeatMap(string showtimeId)
        {
            var showtime = await _context.SuatChieus
                .Include(s => s.PhongChieu)
                    .ThenInclude(p => p.Ghes)
                .Include(s => s.Phim)
                .FirstOrDefaultAsync(s => s.MaSuatChieu == showtimeId);

            if (showtime == null) return NotFound();

            return View(showtime);
        }

        [HttpPost]
        public async Task<IActionResult> LockSeat([FromBody] SeatActionRequest req)
        {
            var success = await _bookingService.LockSeatAsync(req.ShowtimeId, req.SeatId);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> UnlockSeat([FromBody] SeatActionRequest req)
        {
            var success = await _bookingService.UnlockSeatAsync(req.ShowtimeId, req.SeatId);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> Payment(BookingRequestDto req)
        {
            req.UserId = "guest-user"; // Giả định
            var result = await _bookingService.ProcessPaymentAsync(req);
            
            if (result.IsSuccess && result.GiaoDich != null)
            {
                return RedirectToAction("Ticket", new { transactionId = result.GiaoDich.MaGiaoDich });
            }

            TempData["Error"] = result.Error;
            return RedirectToAction("SeatMap", new { showtimeId = req.SuatChieuId });
        }

        public async Task<IActionResult> Ticket(string transactionId)
        {
            var ves = await _context.Ves
                .Include(v => v.Ghe)
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.Phim)
                .Where(v => v.MaGiaoDich == transactionId)
                .ToListAsync();

            if (!ves.Any()) return NotFound();

            return View(ves);
        }
    }

    public class SeatActionRequest 
    {
        public string ShowtimeId { get; set; } = null!;
        public string SeatId { get; set; } = null!;
    }
}
