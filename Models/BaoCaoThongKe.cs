using System;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class BaoCaoThongKe
    {
        [Key]
        public string MaBaoCao { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng nhập tên báo cáo")]
        public string TenBaoCao { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        public decimal TongDoanhThu { get; set; }

        public int TongSoVe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
