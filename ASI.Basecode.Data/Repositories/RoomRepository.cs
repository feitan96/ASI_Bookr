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

    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Room> GetRooms()
        {
            return this.GetDbSet<Room>();
        }

        public bool RoomExists(string roomName)
        {
            return this.GetDbSet<Room>().Any(x => x.Name == roomName);
        }

        public void AddRoom(Room room)
        {
            this.GetDbSet<Room>().Add(room);
            UnitOfWork.SaveChanges();
        }

        public void UpdateRoomInfo(Room room)
        {
            this.GetDbSet<Room>().Update(room);
            UnitOfWork.SaveChanges();
        }

        public void DeleteRoom(Room room)
        {
            this.GetDbSet<Room>().Remove(room);
            UnitOfWork.SaveChanges();
        }
    }
}
