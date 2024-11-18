using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class RecurringBooking
    {
        public int RecurringId { get; set; }
        public int BookingId { get; set; }
        public string Day { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
