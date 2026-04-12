using System;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class ShowtimeCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập Mã Suất Chiếu")]
        [StringLength(20, ErrorMessage = "Mã Suất Chiếu không được dài quá 20 ký tự")]
        [RegularExpression(@"^SC-[-A-Za-z0-9]+$", ErrorMessage = "Mã suất chiếu phải bắt đầu bằng 'SC-' và chỉ chứa chữ/số/gạch ngang (Ví dụ: SC-01)")]
        public string MaSuatChieu { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng chọn Phim")]
        public string PhimId { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng chọn Phòng chiếu")]
        public string PhongChieuId { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng chọn Giờ bắt đầu")]
        public DateTime GioBatDau { get; set; }
    }
}
