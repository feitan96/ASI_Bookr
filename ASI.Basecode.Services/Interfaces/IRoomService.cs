using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using static ASI.Basecode.Resources.Constants.Enums;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Services.ServiceModels.RoomViewModel;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomService
    {
        #nullable enable
        //CREATE
        int AddRoom(RoomViewModel model, int id); //return the generate roomId

        //READ
        PaginatedList<RoomViewModel> GetPagedRooms(string roomName, string type, string location, int? capacity, List<int> amenities, int pageIndex, int pageSize);
        List<RoomViewModel> GetRooms();
        Room? GetRoomModelById(int roomId);
        RoomViewModel? GetRoomById(int roomId);
        RoomViewModel? GetRoomByName(string roomName, bool fuzzyMatching);
        List<RoomViewModel> GetRoomsByFilter(RoomType? roomType, int? capacity, Location? location);
        //List<RoomViewModel> GetRoomsByAmenities(string amenity, bool fuzzyMatching);
        //List<RoomViewModel> GetRoomsByDate(DateTime? date);
    
        //UPDATE
        void UpdateRoomInfo(RoomViewModel model, int userId);

        //DELETE
        void SoftDeleteRoom(int roomId);
        void HardDeleteRoom(int roomId);

        //OTHERS
        //bool IsRoomAvailableDate(int room, DateTime dateTime);
        //List<string>? ListAmenitiesByRoomId(int roomId);
        //List<string>? ListAmenitiesByRoomName(string roomName, bool fuzzyMatching);
        //List<DateTime>? GetRoomBookingDatesByRoomId(int roomId); //Implement once booking implementation is done
        //List<DateTime>? GetRoomBookingDatesByRoomName(string roomName, bool fuzzyMatching); //Implement once booking implementation is done
        //List<User>? GetBookedUsersByRoomId(int roomId);
        //List<User>? GetBookedUsersByRoomName(string roomName, bool fuzzyMatching);
        //List<string>? GetBookedUsersNameByRoomId(int roomId);
        //List<string>? GetBookedUsersNameByRoomName(string roomName, bool fuzzyMatching);

    }   
}
