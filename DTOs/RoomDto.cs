using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class RoomCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã phòng")]
        public string MaPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        public string TenPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn Loại Phòng")]
        public string LoaiPhong { get; set; } = null!;
        
        [Range(1, 26, ErrorMessage = "Số hàng từ 1 đến 26 (A-Z)")]
        [Required(ErrorMessage = "Vui lòng nhập số hàng")]
        public int SoHang { get; set; }
        
        [Range(1, 40, ErrorMessage = "Số ghế/hàng không quá 40")]
        [Required(ErrorMessage = "Vui lòng nhập số ghế/hàng")]
        public int SoGheMoiHang { get; set; }
    }
}
