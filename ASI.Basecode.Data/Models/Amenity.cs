using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Amenity
    {
        public Amenity()
        {
            RoomAmenities = new HashSet<RoomAmenity>();
        }

        public int AmenityId { get; set; }
        public string AmenityName { get; set; }

        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
}
