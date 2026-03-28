using MiniCinema.Models;
using System;

namespace MiniCinema.DTOs
{
    public class SeatDisplayDto
    {
        public string MaGhe { get; set; } = null!;
        public LoaiGhe LoaiGhe { get; set; }
        public TrangThaiGhe TrangThai { get; set; }
        public decimal GiaTien { get; set; } // Giá đã cộng phụ thu
    }
}
