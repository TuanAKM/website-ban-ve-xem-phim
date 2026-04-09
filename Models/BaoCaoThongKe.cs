using System;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class BaoCaoThongKe
    {
        [Key]
        public string MaBaoCao { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string TenBaoCao { get; set; } = null!;

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        public decimal TongDoanhThu { get; set; }

        public int TongSoVe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
