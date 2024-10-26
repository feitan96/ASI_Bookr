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
        void UpdateRoomInfo(Room room);
        void DeleteRoom(Room room);
    }
}
