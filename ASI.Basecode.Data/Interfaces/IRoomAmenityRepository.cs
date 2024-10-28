using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRoomAmenityRepository
    {
        IQueryable<RoomAmenity> GetRoomAmenities();
        bool RoomAmenityExists(int roomId, int amenityId);
        void AddRoomAmenity(RoomAmenity roomAmenity);
        void AddRoomAmenities(List<RoomAmenity> roomAmenities);
        void UpdateRoomAmenity(RoomAmenity roomAmenity);
        void DeleteRoomAmenity(RoomAmenity roomAmenity);
    }
}
