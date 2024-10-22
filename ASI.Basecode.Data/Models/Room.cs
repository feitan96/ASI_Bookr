using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
        }

        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public int? Capacity { get; set; }
        public string Location { get; set; }
        public string Amenities { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
