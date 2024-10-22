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

        public void UpdateRoomInfo(int? roomId, string? roomName, string? description, string? roomType, string? image, int? capacity, string? location, List<string>? amenities)
        {
            if (roomId == null)
            {
                throw new ArgumentNullException(nameof(roomId), "Room ID cannot be null.");
            }

            // Fetch the room from the database
            var room = this.GetDbSet<Room>().Find(roomId);

            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            // Update the fields only if the parameter is not null
            if (!string.IsNullOrEmpty(roomName))
            {
                room.Name = roomName;
            }

            if (!string.IsNullOrEmpty(description))
            {
                room.Description = description;
            }

            if (!string.IsNullOrEmpty(roomType))
            {
                room.Type = roomType;
            }

            if (!string.IsNullOrEmpty(image))
            {
                room.Image = image;
            }

            if (capacity.HasValue)
            {
                room.Capacity = capacity.Value;
            }

            if (!string.IsNullOrEmpty(location))
            {
                room.Location = location;
            }

            if (amenities != null && amenities.Any())
            {
                room.Amenities = string.Join(", ", amenities); // Join list into a comma-separated string
            }

            // Save the changes to the database
            UnitOfWork.SaveChanges(); // Or DbContext.SaveChanges()
        }

        public void HardDeleteRoom(int roomId)
        {
            // Retrieve the room from the database based on roomId
            var room = this.GetDbSet<Room>().FirstOrDefault(x => x.RoomId == roomId);

            // Check if the room exists
            if (room == null)
            {
                // Optionally handle the case where the room is not found
                throw new Exception("Room not found");
            }

            // Remove the room from the database
            this.GetDbSet<Room>().Remove(room);

            // Commit the changes to the database
            UnitOfWork.SaveChanges();
        }

        //public void SoftDeleteRoom(int roomId)
        //{
        //    // Retrieve the room from the database based on roomId
        //    var room = this.GetDbSet<Room>().FirstOrDefault(x => x.Id == roomId);

        //    // Check if the room exists
        //    if (room == null)
        //    {
        //        // Optionally handle the case where the room is not found
        //        throw new Exception("Room not found");
        //    }

        //    // Mark the room as deleted (soft delete)
        //    room.Deleted = true;

        //    // Commit the changes to the database
        //    UnitOfWork.SaveChanges();
        //}

    }
}
