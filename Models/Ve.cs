using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCinema.Models
{
    public class Ve
    {
        [Key]
        public string MaVe { get; set; } = null!;
        public decimal GiaVe { get; set; }
        public TrangThaiVe TrangThaiVe { get; set; }
        public decimal ThueVAT { get; set; } // e.g. 0.08 for 8%
        
        [Required]
        public string GheId { get; set; } = null!;
        [ForeignKey("GheId")]
        public Ghe? Ghe { get; set; }

        [Required]
        public string SuatChieuId { get; set; } = null!;
        [ForeignKey("SuatChieuId")]
        public SuatChieu? SuatChieu { get; set; }

        [Required]
        public string MaGiaoDich { get; set; } = null!;
        [ForeignKey("MaGiaoDich")]
        public GiaoDich? GiaoDich { get; set; }
    }
}
