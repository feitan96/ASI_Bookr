using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using static ASI.Basecode.Resources.Constants.Enums;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomService
    {
        #nullable enable
        //CREATE
        void AddRoom(RoomViewModel model);

        //READ
        List<Room> GetRooms();
        Room? GetRoomById(int roomId);
        Room? GetRoomByName(string roomName, bool fuzzyMatching);
        List<Room> GetRoomsByFilter(RoomType? roomType, int? capacity, Location? location);
        List<Room> GetRoomsByAmenities(string amenity, bool fuzzyMatching);
        List<Room> GetRoomsByDate(DateTime? date);
    
        //UPDATE
        void UpdateRoomInfo(int? roomId, string? roomName, string? description, RoomType? roomType, string? image, int? capacity, Location? location, List<string>? amenities);

        //DELETE
        //void SoftDeleteRoom(int roomId);
        void HardDeleteRoom(int roomId);

        //OTHERS

        bool IsRoomAvailableDate(int room, DateTime dateTime);
        List<string>? ListAmenitiesByRoomId(int roomId);
        List<string>? ListAmenitiesByRoomName(string roomName, bool fuzzyMatching);
        List<DateTime?>? GetRoomBookingDatesByRoomId(int roomId); //Implement once booking implementation is done
        List<DateTime?>? GetRoomBookingDatesByRoomName(string roomName, bool fuzzyMatching); //Implement once booking implementation is done
        List<User>? GetBookedUsersByRoomId(int roomId);
        List<User>? GetBookedUsersByRoomName(string roomName, bool fuzzyMatching);
        List<string>? GetBookedUsersNameByRoomId(int roomId);
        List<string>? GetBookedUsersNameByRoomName(string roomName, bool fuzzyMatching);

    }
}
