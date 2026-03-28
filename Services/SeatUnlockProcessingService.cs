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
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<SeatHub>>();

            var timeLimit = DateTime.Now.AddMinutes(-5);
            
            var expiredSeats = await context.Ghes
                .Where(g => g.TrangThai == TrangThaiGhe.DangChon && g.ThoiGianKhoa < timeLimit)
                .ToListAsync();

            if (expiredSeats.Any())
            {
                foreach (var ghe in expiredSeats)
                {
                    ghe.TrangThai = TrangThaiGhe.Trong;
                    ghe.ThoiGianKhoa = null;
                    await hubContext.Clients.All.SendAsync("SeatStatusChanged", ghe.MaGhe, "Trong");
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
