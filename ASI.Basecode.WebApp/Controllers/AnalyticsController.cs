using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IBookingService _bookingService;

        public AnalyticsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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

            var viewModel = new AnalyticsViewModel
            {
                WeeklyRoomUsage = weeklyRoomUsage
            };

            return View(viewModel);
        }


    }
}
