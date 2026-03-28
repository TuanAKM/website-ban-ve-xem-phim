using Microsoft.EntityFrameworkCore;
using MiniCinema.Data;
using MiniCinema.DTOs;
using MiniCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCinema.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string? Error)> CreateShowtimeAsync(ShowtimeCreateDto dto)
        {
            var phim = await _context.Phims.FindAsync(dto.PhimId);
            if (phim == null) return (false, "Phim không tồn tại.");

            var phongChieu = await _context.PhongChieus.FindAsync(dto.PhongChieuId);
            if (phongChieu == null) return (false, "Phòng chiếu không tồn tại.");

            var gioKetThuc = dto.GioBatDau.AddMinutes(phim.ThoiLuong).AddMinutes(15);

            var isOverlap = await _context.SuatChieus
                .AnyAsync(s => s.PhongChieuId == dto.PhongChieuId &&
                               s.GioBatDau < gioKetThuc &&
                               s.GioKetThuc > dto.GioBatDau);

            if (isOverlap) return (false, "Lịch chiếu phòng này bị trùng.");

            var suatChieu = new SuatChieu
            {
                MaSuatChieu = dto.MaSuatChieu,
                GioBatDau = dto.GioBatDau,
                GioKetThuc = gioKetThuc,
                ThoiGianDonDep = 15,
                TrangThaiBanVe = true,
                PhimId = dto.PhimId,
                PhongChieuId = dto.PhongChieuId
            };

            await _context.SuatChieus.AddAsync(suatChieu);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<IEnumerable<SuatChieu>> GetAvailableShowtimesAsync(string phimId)
        {
            var limitTime = DateTime.Now.AddMinutes(-15);
            return await _context.SuatChieus
                .Include(s => s.PhongChieu)
                .Where(s => s.PhimId == phimId && s.TrangThaiBanVe == true && s.GioBatDau >= limitTime)
                .OrderBy(s => s.GioBatDau)
                .ToListAsync();
        }
    }
}
