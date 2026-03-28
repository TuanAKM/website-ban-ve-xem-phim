using MiniCinema.DTOs;
using MiniCinema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCinema.Services
{
    public interface IShowtimeService
    {
        Task<(bool IsSuccess, string? Error)> CreateShowtimeAsync(ShowtimeCreateDto dto);
        Task<IEnumerable<SuatChieu>> GetAvailableShowtimesAsync(string phimId);
    }
}
