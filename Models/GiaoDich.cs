using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCinema.Models
{
    public class GiaoDich
    {
        [Key]
        public string MaGiaoDich { get; set; } = null!;
        public DateTime NgayGiaoDich { get; set; }
        public decimal TongTien { get; set; }
        public string? PhuongThucTT { get; set; }
        public bool TrangThai { get; set; }

        
        [Required]
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public ICollection<Ve>? Ves { get; set; }
    }
}
