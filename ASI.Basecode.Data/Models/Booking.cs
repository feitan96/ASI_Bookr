using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Booking
    {
        public Booking()
        {
            RecurringBookings = new HashSet<RecurringBooking>();
        }

        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingStartDate { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? ApproveDisapproveBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsRecurring { get; set; }
        public string Title { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<RecurringBooking> RecurringBookings { get; set; }
    }
}
