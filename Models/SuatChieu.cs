using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCinema.Models
{
    public class SuatChieu
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập mã suất chiếu")]
        public string MaSuatChieu { get; set; } = null!;
        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }
        public int ThoiGianDonDep { get; set; } = 15;
        public bool TrangThaiBanVe { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn Phim")]
        public string PhimId { get; set; } = null!;
        [ForeignKey("PhimId")]
        public Phim? Phim { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phòng chiếu")]
        public string PhongChieuId { get; set; } = null!;
        [ForeignKey("PhongChieuId")]
        public PhongChieu? PhongChieu { get; set; }
        
        public ICollection<Ve>? Ves { get; set; }
    }
}
