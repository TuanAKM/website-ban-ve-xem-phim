using MiniCinema.DTOs;
using MiniCinema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCinema.Services
{
    public interface IBookingService
    {
        Task<bool> LockSeatAsync(string suatChieuId, string maGhe);
        Task<bool> UnlockSeatAsync(string suatChieuId, string maGhe);
        Task<(bool IsSuccess, string? Error, GiaoDich? GiaoDich)> ProcessPaymentAsync(BookingRequestDto bookingDto);
    }
}
