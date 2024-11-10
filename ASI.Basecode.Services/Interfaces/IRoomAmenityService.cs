using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomAmenityService
    {
#nullable enable
        //CREATE
        void AddRoomAmenity(RoomAmenityViewModel model);
        void AddRoomAmenity(int roomId, int amenityId);

        //READ
        RoomAmenityViewModel GetRoomAmenity(int Id);
        RoomAmenityViewModel GetRoomAmenity(int roomId, int amenityId);
        List<RoomAmenityViewModel> GetRoomAmenities(int roomId); //Get all the room-amenity mappings of a specific roomId

        //UPDATE
        void UpdateRoomAmenityInfo(RoomAmenityViewModel model);

        //DELETE
        void DeleteRoomAmenity(int id);
        void DeleteRoomAmenity(int roomId, int amenityId);

        //OTHERS

    }
}
