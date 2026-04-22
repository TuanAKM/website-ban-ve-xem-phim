using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCinema.Models
{
    public class Ghe
    {
        [Key]
        public string MaGhe { get; set; } = null!;
        public LoaiGhe LoaiGhe { get; set; }
        public TrangThaiGhe TrangThai { get; set; }
        public DateTime? ThoiGianKhoa { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn Phòng chiếu")]
        public string PhongChieuId { get; set; } = null!;
        [ForeignKey("PhongChieuId")]
        public PhongChieu? PhongChieu { get; set; }
    }
}
