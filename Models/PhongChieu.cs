using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class PhongChieu
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập mã phòng")]
        public string MaPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        public string TenPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn Loại Phòng")]
        public string LoaiPhong { get; set; } = null!;
        public int TongSoGhe { get; set; }
        
        public ICollection<Ghe>? Ghes { get; set; }
        public ICollection<SuatChieu>? SuatChieus { get; set; }
    }
}
