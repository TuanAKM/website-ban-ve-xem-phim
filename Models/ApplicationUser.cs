using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MiniCinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public ICollection<GiaoDich>? GiaoDichs { get; set; }
    }
}
