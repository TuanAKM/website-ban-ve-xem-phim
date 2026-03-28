using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class BookingRequestDto
    {
        [Required]
        public string SuatChieuId { get; set; } = null!;
        [Required]
        public List<string> SelectedSeatIds { get; set; } = new List<string>();
        [Required]
        public string UserId { get; set; } = null!;
    }
}
