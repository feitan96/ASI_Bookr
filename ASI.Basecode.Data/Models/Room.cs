using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
            Images = new HashSet<Image>();
        }

        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public int? Capacity { get; set; }
        public string Location { get; set; }
        public string Amenities { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
