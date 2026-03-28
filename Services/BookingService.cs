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

        public BookingService(ApplicationDbContext context, IHubContext<SeatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<bool> LockSeatAsync(string suatChieuId, string maGhe)
        {
            var ghe = await _context.Ghes.FindAsync(maGhe);
            if (ghe == null || ghe.TrangThai == TrangThaiGhe.DaBan || ghe.TrangThai == TrangThaiGhe.Hong) return false;

            if (ghe.TrangThai == TrangThaiGhe.DangChon)
            {
                if (ghe.ThoiGianKhoa.HasValue && (DateTime.Now - ghe.ThoiGianKhoa.Value).TotalMinutes < 5) return false;
            }

            ghe.TrangThai = TrangThaiGhe.DangChon;
            ghe.ThoiGianKhoa = DateTime.Now;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("SeatStatusChanged", maGhe, "DangChon");
            return true;
        }

        public async Task<bool> UnlockSeatAsync(string suatChieuId, string maGhe)
        {
            var ghe = await _context.Ghes.FindAsync(maGhe);
            if (ghe == null || ghe.TrangThai != TrangThaiGhe.DangChon) return false;

            ghe.TrangThai = TrangThaiGhe.Trong;
            ghe.ThoiGianKhoa = null;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("SeatStatusChanged", maGhe, "Trong");
            return true;
        }

        public async Task<(bool IsSuccess, string? Error, GiaoDich? GiaoDich)> ProcessPaymentAsync(BookingRequestDto bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal tongTien = 0;
                var ves = new List<Ve>();
                var maGd = Guid.NewGuid().ToString();

                foreach (var seatId in bookingDto.SelectedSeatIds)
                {
                    var ghe = await _context.Ghes.FindAsync(seatId);
                    if (ghe == null || ghe.TrangThai != TrangThaiGhe.DangChon)
                        return (false, $"Ghế {seatId} không hợp lệ hoặc đã bị người khác mua.", null);

                    decimal giaVe = 50000; // Base
                    if (ghe.LoaiGhe == LoaiGhe.Vip) giaVe += 20000;
                    if (ghe.LoaiGhe == LoaiGhe.Doi) giaVe += 50000;

                    tongTien += giaVe;

                    ghe.TrangThai = TrangThaiGhe.DaBan;
                    ghe.ThoiGianKhoa = null;

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
                    PhuongThucTT = "Online",
                    UserId = bookingDto.UserId
                };

                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.Ves.AddRangeAsync(ves);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                foreach (var seatId in bookingDto.SelectedSeatIds)
                {
                    await _hubContext.Clients.All.SendAsync("SeatStatusChanged", seatId, "DaBan");
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
