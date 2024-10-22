using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRoomRepository
    {
        IQueryable<Room> GetRooms();
        bool RoomExists(string roomName);
        void AddRoom(Room room);
        void UpdateRoomInfo(int? roomId, string? roomName, string? description, string? roomType, string? image, int? capacity, string? location, List<string>? amenities);
        void HardDeleteRoom(int roomId);
        //void SoftDeleteRoom(int? roomId);
    }
}
