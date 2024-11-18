using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            TodayBookings = new List<BookingViewModel>();
            RoomUsageStatistics = new List<RoomUsageStatisticsViewModel>();
        }

        public List<BookingViewModel> TodayBookings { get; set; }
        public List<RoomUsageStatisticsViewModel> RoomUsageStatistics { get; set; }
        public DateTime CurrentDate { get; set; }
    }

    public class RoomUsageStatisticsViewModel
    {
        public string RoomName { get; set; }
        public int BookingFrequency { get; set; }
        public double TotalUsageHours { get; set; }
    }

}
