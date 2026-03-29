using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MiniCinema.Services
{
    public class SeatLockInfo
    {
        public string ShowtimeId { get; set; } = null!;
        public string SeatId { get; set; } = null!;
        public DateTime LockedAt { get; set; }
    }

    public interface ISeatLockService
    {
        bool LockSeat(string showtimeId, string seatId);
        bool UnlockSeat(string showtimeId, string seatId);
        List<SeatLockInfo> GetLockedSeats(string showtimeId);
        List<SeatLockInfo> GetExpiredLocks(int minutes);
    }

    public class SeatLockService : ISeatLockService
    {
        // Key: ShowtimeId_SeatId
        private readonly ConcurrentDictionary<string, SeatLockInfo> _lockedSeats = new();

        public bool LockSeat(string showtimeId, string seatId)
        {
            var key = $"{showtimeId}_{seatId}";
            
            // Allow relocking if already locked but expired. 
            // In our case, the background service should handle unlocks anyway.
            // But just TryAdd is simple enough.
            var info = new SeatLockInfo
            {
                ShowtimeId = showtimeId,
                SeatId = seatId,
                LockedAt = DateTime.Now
            };

            return _lockedSeats.TryAdd(key, info);
        }

        public bool UnlockSeat(string showtimeId, string seatId)
        {
            var key = $"{showtimeId}_{seatId}";
            return _lockedSeats.TryRemove(key, out _);
        }

        public List<SeatLockInfo> GetLockedSeats(string showtimeId)
        {
            return _lockedSeats.Values.Where(v => v.ShowtimeId == showtimeId).ToList();
        }

        public List<SeatLockInfo> GetExpiredLocks(int minutes)
        {
            var timeLimit = DateTime.Now.AddMinutes(-minutes);
            return _lockedSeats.Values.Where(v => v.LockedAt < timeLimit).ToList();
        }
    }
}
