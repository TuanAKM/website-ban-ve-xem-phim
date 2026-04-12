using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Hubs;
using MiniCinema.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCinema.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SeatHub> _hubContext;
        private readonly ISeatLockService _seatLockService;

        public BookingService(ApplicationDbContext context, IHubContext<SeatHub> hubContext, ISeatLockService seatLockService)
        {
            _context = context;
            _hubContext = hubContext;
            _seatLockService = seatLockService;
        }

        public async Task<bool> LockSeatAsync(string suatChieuId, string maGhe)
        {
            var ghe = await _context.Ghes.FindAsync(maGhe);
            if (ghe == null || ghe.TrangThai == TrangThaiGhe.Hong) return false;

            var isSold = await _context.Ves.AnyAsync(v => v.SuatChieuId == suatChieuId && v.GheId == maGhe && v.TrangThaiVe != TrangThaiVe.Huy);
            if (isSold) return false;

            bool success = _seatLockService.LockSeat(suatChieuId, maGhe);
            if (success)
            {
                await _hubContext.Clients.Group(suatChieuId).SendAsync("SeatStatusChanged", maGhe, "DangChon");
            }
            return success;
        }

        public async Task<bool> UnlockSeatAsync(string suatChieuId, string maGhe)
        {
            var success = _seatLockService.UnlockSeat(suatChieuId, maGhe);
            if (success)
            {
                await _hubContext.Clients.Group(suatChieuId).SendAsync("SeatStatusChanged", maGhe, "Trong");
            }
            return success;
        }

        public async Task<(bool IsSuccess, string? Error, GiaoDich? GiaoDich)> ProcessPaymentAsync(BookingRequestDto bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal tongTien = 0;
                var ves = new List<Ve>();
                var maGd = Guid.NewGuid().ToString();

                var suatChieu = await _context.SuatChieus.FindAsync(bookingDto.SuatChieuId);
                if (suatChieu == null) return (false, "Suất chiếu không tồn tại.", null);

                decimal maxMultiplier = 0.0m;
                if (suatChieu.GioBatDau.Hour >= 18 && suatChieu.GioBatDau.Hour <= 23) maxMultiplier = Math.Max(maxMultiplier, 0.15m);
                var holidays = new[] { (1, 1), (30, 4), (1, 5), (2, 9), (24, 12), (31, 12) };
                if (holidays.Contains((suatChieu.GioBatDau.Day, suatChieu.GioBatDau.Month))) maxMultiplier = Math.Max(maxMultiplier, 0.30m);
                else if (suatChieu.GioBatDau.DayOfWeek == DayOfWeek.Friday ||
                         suatChieu.GioBatDau.DayOfWeek == DayOfWeek.Saturday ||
                         suatChieu.GioBatDau.DayOfWeek == DayOfWeek.Sunday) maxMultiplier = Math.Max(maxMultiplier, 0.20m);
                
                decimal multiplier = 1.0m + maxMultiplier;

                foreach (var seatId in bookingDto.SelectedSeatIds)
                {
                    var ghe = await _context.Ghes.FindAsync(seatId);
                    if (ghe == null || ghe.TrangThai == TrangThaiGhe.Hong)
                        return (false, $"Ghế {seatId} không khả dụng.", null);

                    var isSold = await _context.Ves.AnyAsync(v => v.SuatChieuId == bookingDto.SuatChieuId && v.GheId == seatId && v.TrangThaiVe != TrangThaiVe.Huy);
                    if (isSold) return (false, $"Ghế {seatId} đã bị người khác mua. Giao dịch thất bại.", null);

                    _seatLockService.UnlockSeat(bookingDto.SuatChieuId, seatId);

                    decimal basePrice = 50000; // Base
                    if (ghe.LoaiGhe == LoaiGhe.Vip) basePrice += 20000;
                    if (ghe.LoaiGhe == LoaiGhe.Doi) basePrice += 50000;
                    
                    decimal giaVe = basePrice * multiplier;

                    tongTien += giaVe;

                    var ve = new Ve
                    {
                        MaVe = Guid.NewGuid().ToString(),
                        GiaVe = giaVe,
                        TrangThaiVe = TrangThaiVe.DaBan,
                        ThueVAT = 0.08m,
                        GheId = ghe.MaGhe,
                        SuatChieuId = bookingDto.SuatChieuId,
                        MaGiaoDich = maGd
                    };
                    ves.Add(ve);
                }

                var giaoDich = new GiaoDich
                {
                    MaGiaoDich = maGd,
                    NgayGiaoDich = DateTime.Now,
                    TongTien = tongTien,
                    PhuongThucTT = "Offline",
                    UserId = bookingDto.UserId,
                    TrangThai = true
                };

                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.Ves.AddRangeAsync(ves);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                foreach (var seatId in bookingDto.SelectedSeatIds)
                {
                    await _hubContext.Clients.Group(bookingDto.SuatChieuId).SendAsync("SeatStatusChanged", seatId, "DaBan");
                }

                return (true, null, giaoDich);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Lỗi thanh toán: " + ex.Message, null);
            }
        }
    }
}
