using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class Phim
    {
        [Key]
        public string MaPhim { get; set; } = null!;
        [Required]
        public string TenPhim { get; set; } = null!;
        public int ThoiLuong { get; set; } // minutes
        public string? TheLoai { get; set; }
        public string? NhanDoTuoi { get; set; }
        public string? DinhDang { get; set; }
        
        public ICollection<SuatChieu>? SuatChieus { get; set; }
    }
}
