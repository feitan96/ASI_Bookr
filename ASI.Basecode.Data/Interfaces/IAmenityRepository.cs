using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAmenityRepository
    {
        IQueryable<Amenity> GetAmenities();
        bool AmenityExists(int amenityId);
        void AddAmenity(Amenity amenity);
        void AddAmenities(List<Amenity> amenities);
        void UpdateAmenity(Amenity amenity);
        void DeleteAmenity(Amenity amenity);
    }
}
