using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int? AdminId { get; set; }
        public int? SuperadminId { get; set; }
        public DateTime BookingCheckInDateTime { get; set; }
        public DateTime BookingCheckOutDateTime { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Room Room { get; set; }
        public virtual Superadmin Superadmin { get; set; }
        public virtual User User { get; set; }
    }
}
