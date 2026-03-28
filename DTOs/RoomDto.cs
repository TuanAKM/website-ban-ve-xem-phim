using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class RoomCreateDto
    {
        [Required]
        public string MaPhong { get; set; } = null!;
        [Required]
        public string TenPhong { get; set; } = null!;
        public string? LoaiPhong { get; set; }
        
        [Range(1, 26, ErrorMessage = "Số hàng từ 1 đến 26 (A-Z)")]
        public int SoHang { get; set; }
        
        [Range(1, 40, ErrorMessage = "Số ghế/hàng không quá 40")]
        public int SoGheMoiHang { get; set; }
    }
}
