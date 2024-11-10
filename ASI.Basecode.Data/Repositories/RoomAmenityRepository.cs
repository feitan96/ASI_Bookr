using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class RoomAmenityRepository: BaseRepository, IRoomAmenityRepository
    {

        public RoomAmenityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<RoomAmenity> GetRoomAmenities()
        {
            return this.GetDbSet<RoomAmenity>().Include(roomAmenity => roomAmenity.Amenity);
        }

        public bool RoomAmenityExists(int roomId, int amenityId)
        {
            return this.GetDbSet<RoomAmenity>().Any(x => (x.RoomId == roomId && x.AmenityId == amenityId));
        }

        public void AddRoomAmenity(RoomAmenity roomAmenity)
        {
            this.GetDbSet<RoomAmenity>().Add(roomAmenity);
            UnitOfWork.SaveChanges();
        }

        public void AddRoomAmenities(List<RoomAmenity> roomAmenity)
        {
            roomAmenity.ForEach(x =>
                {
                    this.GetDbSet<RoomAmenity>().Add(x);
                }
            );
            UnitOfWork.SaveChanges();
        }

        public void UpdateRoomAmenity(RoomAmenity roomAmenity)
        {
            this.GetDbSet<RoomAmenity>().Update(roomAmenity);
            UnitOfWork.SaveChanges();   
        }

        public void DeleteRoomAmenity(RoomAmenity roomAmenity)
        {
            this.GetDbSet<RoomAmenity>().Remove(roomAmenity);
            UnitOfWork.SaveChanges();
        }
    }
}
