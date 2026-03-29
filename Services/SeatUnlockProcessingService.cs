using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniCinema.Data;
using MiniCinema.Hubs;
using MiniCinema.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCinema.Services
{
    public class SeatUnlockProcessingService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public SeatUnlockProcessingService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessUnlockSeats();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessUnlockSeats()
        {
            using var scope = _services.CreateScope();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<SeatHub>>();
            var seatLockService = scope.ServiceProvider.GetRequiredService<ISeatLockService>();

            var expiredLocks = seatLockService.GetExpiredLocks(5);

            foreach (var lockInfo in expiredLocks)
            {
                // Xóa khỏi cache
                seatLockService.UnlockSeat(lockInfo.ShowtimeId, lockInfo.SeatId);
                
                // Nhả ghế cho những người đang xem cùng suất chiếu
                await hubContext.Clients.Group(lockInfo.ShowtimeId).SendAsync("SeatStatusChanged", lockInfo.SeatId, "Trong");
            }
        }
    }
}
