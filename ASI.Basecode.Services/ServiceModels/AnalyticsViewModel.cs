using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AnalyticsViewModel
    {
        public AnalyticsViewModel()
        {
            RoomUsageStatistics = new List<RoomUsageStatisticsViewModel>();
            WeeklyRoomUsage = new List<WeeklyRoomUsageViewModel>();
        }

        public List<RoomUsageStatisticsViewModel> RoomUsageStatistics { get; set; }
        public List<WeeklyRoomUsageViewModel> WeeklyRoomUsage { get; set; }
        public MostUsedRoomViewModel MostUsedRoom { get; set; }
    }

    public class WeeklyRoomUsageViewModel
    {
        public string RoomName { get; set; }
        public List<int> WeeklyBookingFrequency { get; set; }
        public List<string> Weeks { get; set; }
    }

    public class MostUsedRoomViewModel
    {
        public string RoomName { get; set; }
        public int BookingFrequency { get; set; }
        public double TotalUsageHours { get; set; }
        public string PeakUsageHours { get; set; }
    }
}
