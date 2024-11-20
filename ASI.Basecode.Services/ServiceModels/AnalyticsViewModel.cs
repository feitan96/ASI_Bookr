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
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public int TotalAdmins { get; set; }
        public List<UserTrend> UserTrends { get; set; }
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

    public class UserTrend
    {
        public string Date { get; set; }
        public int UserCount { get; set; }
        public int AdminCount { get; set; }
    }

}
