using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Superadmin
    {
        public Superadmin()
        {
            Bookings = new HashSet<Booking>();
        }

        public int SuperadminId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
