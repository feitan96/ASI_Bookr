using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public AnalyticsController(IBookingService bookingService, IUserService userService)
        {
            _bookingService = bookingService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var bookings = _bookingService.GetBookings()
                .Where(b => b.Status == "Approved")
                .ToList();

            // Determine weekly range
            var startDate = bookings.Min(b => b.BookingStartDate).Date;
            var endDate = bookings.Max(b => b.BookingStartDate).Date;
            var weeks = Enumerable.Range(0, (endDate - startDate).Days / 7 + 1)
                .Select(i => startDate.AddDays(i * 7))
                .ToList();

            // Compute weekly room usage (in hours) per room
            var weeklyRoomUsage = bookings
                .GroupBy(b => b.Room.Name)
                .Select(group => new WeeklyRoomUsageViewModel
                {
                    RoomName = group.Key,
                    WeeklyBookingFrequency = weeks.Select(weekStart =>
                        group
                            .Where(b => b.BookingStartDate >= weekStart && b.BookingStartDate < weekStart.AddDays(7))
                            .Sum(b => (b.CheckOutTime - b.CheckInTime).TotalHours)
                    ).Select(hours => (int)Math.Round(hours)).ToList(),
                    Weeks = weeks.Select(w => w.ToString("MMM dd")).ToList()
                }).ToList();

            // Compute most used room
            var mostUsedRoomGroup = bookings
    .GroupBy(b => b.Room)
    .OrderByDescending(group => group.Count())
    .FirstOrDefault();

            var mostUsedRoom = new MostUsedRoomViewModel();
            if (mostUsedRoomGroup != null)
            {
                var room = mostUsedRoomGroup.Key;
                var roomBookings = mostUsedRoomGroup.ToList();

                // Safely group by time slots (handle default TimeSpan values)
                var timeSlots = roomBookings
                    .Where(b => b.CheckInTime != TimeSpan.Zero && b.CheckOutTime != TimeSpan.Zero)
                    .GroupBy(b => $"{b.CheckInTime:hh\\:mm} - {b.CheckOutTime:hh\\:mm}")
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                mostUsedRoom = new MostUsedRoomViewModel
                {
                    RoomName = room.Name,
                    BookingFrequency = roomBookings.Count,
                    TotalUsageHours = roomBookings
                        .Where(b => b.CheckInTime != TimeSpan.Zero && b.CheckOutTime != TimeSpan.Zero)
                        .Sum(b => (b.CheckOutTime - b.CheckInTime).TotalHours),
                    PeakUsageHours = timeSlots?.Key ?? "N/A"
                };
            }

            var users = _userService.GetAllUser();
            var recentUsers = users.Where(u => u.CreatedDate.HasValue && u.CreatedDate.Value >= DateTime.UtcNow.AddDays(-3)).ToList();
            var totalUsers = users.Count(u => u.Role == "User");
            var totalAdmins = users.Count(u => u.Role == "Admin");

            // Group data by date for Highcharts
            var userTrends = users
    .Where(u => u.CreatedDate.HasValue)
    .GroupBy(u => u.CreatedDate.Value.Date)
    .OrderBy(g => g.Key)
    .Select(g => new UserTrend
    {
        Date = g.Key.ToString("yyyy-MM-dd"),
        UserCount = g.Count(u => u.Role == "User"),
        AdminCount = g.Count(u => u.Role == "Admin")
    })
    .ToList();


            var viewModel = new AnalyticsViewModel
            {
                WeeklyRoomUsage = weeklyRoomUsage,
                MostUsedRoom = mostUsedRoom,
                TotalUsers = totalUsers,
                NewUsers = recentUsers.Count,
                TotalAdmins = totalAdmins,
                UserTrends = userTrends
            };

            return View(viewModel);
        }



    }
}
