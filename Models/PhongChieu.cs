using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class PhongChieu
    {
        [Key]
        public string MaPhong { get; set; } = null!;
        [Required]
        public string TenPhong { get; set; } = null!;
        public string? LoaiPhong { get; set; }
        public int TongSoGhe { get; set; }
        
        public ICollection<Ghe>? Ghes { get; set; }
        public ICollection<SuatChieu>? SuatChieus { get; set; }
    }
}
