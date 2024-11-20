using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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

        public IActionResult Index(TimeFrame timeFrame = TimeFrame.Weekly)
        {
            var bookings = _bookingService.GetBookings()
                .Where(b => b.Status == "Approved")
                .ToList();

            var startDate = bookings.Min(b => b.BookingStartDate).Date;
            var endDate = bookings.Max(b => b.BookingStartDate).Date;

            var roomUsageData = new List<RoomUsageData>();

            switch (timeFrame)
            {
                case TimeFrame.Daily:
                    var days = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(i => startDate.AddDays(i))
                        .ToList();

                    roomUsageData = bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = days.Select(day =>
                                group
                                    .Where(b => b.BookingStartDate.Date == day)
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = days.Select(d => d.ToString("MMM dd")).ToList()
                        }).ToList();
                    break;

                case TimeFrame.Monthly:
                    var months = Enumerable.Range(0, (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1)
                        .Select(i => startDate.AddMonths(i))
                        .ToList();

                    roomUsageData = bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = months.Select(month =>
                                group
                                    .Where(b => b.BookingStartDate.Year == month.Year && b.BookingStartDate.Month == month.Month)
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = months.Select(m => m.ToString("MMM yyyy")).ToList()
                        }).ToList();
                    break;

                default: // Weekly
                    var weeks = Enumerable.Range(0, (endDate - startDate).Days / 7 + 1)
                        .Select(i => startDate.AddDays(i * 7))
                        .ToList();

                    roomUsageData = bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = weeks.Select(weekStart =>
                                group
                                    .Where(b => b.BookingStartDate >= weekStart && b.BookingStartDate < weekStart.AddDays(7))
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = weeks.Select(w => w.ToString("MMM dd")).ToList()
                        }).ToList();
                    break;
            }

            // Compute most used room using the same approach as Dashboard
            var mostUsedRoomGroup = bookings
                .GroupBy(b => b.Room)
                .Select(group => new
                {
                    Room = group.Key,
                    BookingCount = group.Count(),
                    TotalHours = group.Sum(b => (b.CheckOutTime - b.CheckInTime).TotalHours),
                    Bookings = group.ToList()
                })
                .OrderByDescending(g => g.BookingCount)
                .FirstOrDefault();

            var mostUsedRoom = new MostUsedRoomViewModel();
            if (mostUsedRoomGroup != null)
            {
                // Find peak usage hours
                var timeSlots = mostUsedRoomGroup.Bookings
                    .GroupBy(b => $"{b.CheckInTime:hh\\:mm} - {b.CheckOutTime:hh\\:mm}")
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                mostUsedRoom = new MostUsedRoomViewModel
                {
                    RoomName = mostUsedRoomGroup.Room.Name,
                    BookingFrequency = mostUsedRoomGroup.BookingCount,
                    TotalUsageHours = mostUsedRoomGroup.TotalHours,  // Using the direct sum calculation
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
                MostUsedRoom = mostUsedRoom,
                TotalUsers = totalUsers,
                NewUsers = recentUsers.Count,
                TotalAdmins = totalAdmins,
                UserTrends = userTrends,
                SelectedTimeFrame = timeFrame,
                RoomUsage = roomUsageData,
            };

            return View(viewModel);
        }


        public IActionResult ExportToExcel(TimeFrame timeFrame = TimeFrame.Weekly)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var bookings = _bookingService.GetBookings()
                .Where(b => b.Status == "Approved")
                .ToList();

            var users = _userService.GetAllUser();

            using (var package = new ExcelPackage())
            {
                // Room Usage Sheet
                var roomUsageSheet = package.Workbook.Worksheets.Add("Room Usage");
                var roomUsageData = GetRoomUsageData(timeFrame); // Pass the timeFrame parameter

                // Headers for Room Usage
                roomUsageSheet.Cells[1, 1].Value = "Room";
                int col = 2;
                foreach (var label in roomUsageData.FirstOrDefault()?.TimeLabels ?? new List<string>())
                {
                    roomUsageSheet.Cells[1, col].Value = label;
                    col++;
                }

                // Data for Room Usage
                int row = 2;
                foreach (var roomData in roomUsageData)
                {
                    roomUsageSheet.Cells[row, 1].Value = roomData.RoomName;
                    for (int i = 0; i < roomData.UsageFrequency.Count; i++)
                    {
                        roomUsageSheet.Cells[row, i + 2].Value = roomData.UsageFrequency[i];
                    }
                    row++;
                }

                // User Trends Sheet
                var userTrendsSheet = package.Workbook.Worksheets.Add("User Trends");
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

                // Headers for User Trends
                userTrendsSheet.Cells[1, 1].Value = "Date";
                userTrendsSheet.Cells[1, 2].Value = "Users";
                userTrendsSheet.Cells[1, 3].Value = "Admins";

                // Data for User Trends
                row = 2;
                foreach (var trend in userTrends)
                {
                    userTrendsSheet.Cells[row, 1].Value = trend.Date;
                    userTrendsSheet.Cells[row, 2].Value = trend.UserCount;
                    userTrendsSheet.Cells[row, 3].Value = trend.AdminCount;
                    row++;
                }

                // Format both sheets
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    // Format headers
                    var headerRange = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Add borders
                    var dataRange = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];
                    dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Generate the file
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Analytics_Export_{timeFrame}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(content, contentType, fileName);
            }
        }

        private List<RoomUsageData> GetRoomUsageData(TimeFrame timeFrame)
        {
            var bookings = _bookingService.GetBookings()
                .Where(b => b.Status == "Approved")
                .ToList();

            var startDate = bookings.Min(b => b.BookingStartDate).Date;
            var endDate = bookings.Max(b => b.BookingStartDate).Date;

            switch (timeFrame)
            {
                case TimeFrame.Daily:
                    var days = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(i => startDate.AddDays(i))
                        .ToList();

                    return bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = days.Select(day =>
                                group
                                    .Where(b => b.BookingStartDate.Date == day)
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = days.Select(d => d.ToString("MMM dd")).ToList()
                        }).ToList();

                case TimeFrame.Monthly:
                    var months = Enumerable.Range(0, (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1)
                        .Select(i => startDate.AddMonths(i))
                        .ToList();

                    return bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = months.Select(month =>
                                group
                                    .Where(b => b.BookingStartDate.Year == month.Year && b.BookingStartDate.Month == month.Month)
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = months.Select(m => m.ToString("MMM yyyy")).ToList()
                        }).ToList();

                default: // Weekly
                    var weeks = Enumerable.Range(0, (endDate - startDate).Days / 7 + 1)
                        .Select(i => startDate.AddDays(i * 7))
                        .ToList();

                    return bookings
                        .GroupBy(b => b.Room.Name)
                        .Select(group => new RoomUsageData
                        {
                            RoomName = group.Key,
                            UsageFrequency = weeks.Select(weekStart =>
                                group
                                    .Where(b => b.BookingStartDate >= weekStart && b.BookingStartDate < weekStart.AddDays(7))
                                    .Sum(b => (int)Math.Round((b.CheckOutTime - b.CheckInTime).TotalHours))
                            ).ToList(),
                            TimeLabels = weeks.Select(w => w.ToString("MMM dd")).ToList()
                        }).ToList();
            }
        }
    }
}
