using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class AmenityRepository : BaseRepository, IAmenityRepository
    {
        public AmenityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Amenity> GetAmenities()
        {
            return this.GetDbSet<Amenity>();
        }

        public bool AmenityExists(int amenityId)
        {
            return this.GetDbSet<Amenity>().Any(x => x.AmenityId == amenityId);
        }

        public void AddAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Add(amenity);
            UnitOfWork.SaveChanges();
        }

        public void AddAmenities(List<Amenity> amenities)
        {
            amenities.ForEach(x =>
            {
                this.GetDbSet<Amenity>().Add(x);
            }
            );
            UnitOfWork.SaveChanges();
        }

        public void UpdateAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Update(amenity);
            UnitOfWork.SaveChanges();
        }

        public void DeleteAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Remove(amenity);
            UnitOfWork.SaveChanges();
        }

    }
}
