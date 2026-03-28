using System;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class ShowtimeCreateDto
    {
        [Required]
        public string MaSuatChieu { get; set; } = null!;
        [Required]
        public string PhimId { get; set; } = null!;
        [Required]
        public string PhongChieuId { get; set; } = null!;
        [Required]
        public DateTime GioBatDau { get; set; }
    }
}
