using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class Phim
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập mã phim")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Mã phim chỉ được chứa chữ cái, số và dấu gạch ngang (không chứa dấu cách hay ký tự đặc biệt)")]
        public string MaPhim { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng nhập tên phim")]
        [RegularExpression(@"^[\p{L}\p{N}\s\-]+$", ErrorMessage = "Tên phim không được chứa ký tự đặc biệt (!@#$...)")]
        public string TenPhim { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập thời lượng phim")]
        [Range(1, 300, ErrorMessage = "Thời lượng phải từ 1 đến 300 phút")]
        public int ThoiLuong { get; set; } // minutes
        [Required(ErrorMessage = "Vui lòng chọn Thể loại phim")]
        public string TheLoai { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn độ tuổi")]
        [RegularExpression(@"^[a-zA-Z0-9\+]+$", ErrorMessage = "Nhãn độ tuổi không được chứa khoảng trắng hay ký tự đặc biệt")]
        public string NhanDoTuoi { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn Định dạng phim")]
        public string DinhDang { get; set; } = null!;
        
        public ICollection<SuatChieu>? SuatChieus { get; set; }
    }
}
