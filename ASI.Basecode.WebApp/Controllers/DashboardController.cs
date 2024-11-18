using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IBookingService _bookingService;

        public DashboardController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            DateTime today = DateTime.Today;

            // Fetch today's bookings
            var bookings = _bookingService.GetBookings()
                .Where(b => b.BookingStartDate.Date == today && b.Status == "Approved")
                .ToList();

            // Group bookings by room
            var roomUsageStats = bookings
            .GroupBy(b => b.RoomId)
            .Select(group => new RoomUsageStatisticsViewModel
            {
                RoomName = group.First().Room.Name, // Fetch Room Name from the first booking in the group
                BookingFrequency = group.Count(),
            TotalUsageHours = group.Sum(b => (b.CheckOutTime - b.CheckInTime).TotalHours)
                })
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TodayBookings = bookings,
                RoomUsageStatistics = roomUsageStats,
                CurrentDate = today
            };

            return View(viewModel);
        }
    }
}
