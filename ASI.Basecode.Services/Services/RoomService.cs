using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzySharp;
using static ASI.Basecode.Resources.Constants.Enums;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using FuzzySharp.SimilarityRatio;
using System.Data.Entity.Core.Objects.DataClasses;
using LinqKit;

namespace ASI.Basecode.Services.Services
{
    public class RoomService : IRoomService
    {
    #nullable enable

        private readonly IRoomRepository _repository;
        private readonly IRoomAmenityService _roomamenityservice;
        private readonly IAmenityService _amenityservice;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository repository, IMapper mapper, IRoomAmenityService roomAmenityService,
                       IAmenityService amenityService)
        {
            _mapper = mapper;
            _repository = repository;
            _roomamenityservice = roomAmenityService;
            _amenityservice = amenityService;
        }

        #region Create (CRUD)

        // Adds a new room if it doesn't already exist, otherwise throws an exception.
        public int AddRoom(RoomViewModel model, int userId)
        {
            var room = new Room();
            if (!_repository.RoomExists(model.Name))
            {
                _mapper.Map(model, room);
                room.CreatedBy = userId;
                room.UpdatedBy = userId;
                room.IsDeleted = false; //default room is not deleted
                _repository.AddRoom(room); //after pushing change, attempt to get the generate roomId
                return room.RoomId;
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.RoomExists);
            }
        }   

        #endregion


        #region Read (CRUD)

        //Returns all room from database.
        public List<RoomViewModel> GetRooms() {
           var rooms = _repository.GetRooms().Where(x => x.IsDeleted == false).Select(room => new RoomViewModel(room)).ToList();
            rooms.ForEach(room =>
            {
                room.RoomAmenities = _roomamenityservice.GetRoomAmenities(room.RoomId);
                room.RoomAmenities.ForEach(
                    roomAmenity =>
                    {
                        roomAmenity.Amenity = _amenityservice.GetAmenity(roomAmenity.AmenityId);
                    }
                    );
            });
            return rooms;
        }

        // Finds and returns a room by its ID. If no room is found, returns null.
        public RoomViewModel? GetRoomById(int roomId)   
        {
            Room? room = _repository.GetRooms().Where(x => x.RoomId == roomId).FirstOrDefault();
            if (room != null)
            {
                var roomModel = new RoomViewModel(room);

                //Get Amenities
                roomModel.RoomAmenities = _roomamenityservice.GetRoomAmenities(roomId);
                roomModel.RoomAmenities.ForEach(
                    roomAmenity =>
                    {
                        roomAmenity.Amenity = _amenityservice.GetAmenity(roomAmenity.AmenityId);
                    }
                );

                return roomModel;
            }
            else
            {
                return null;
            }
        }

        // Finds and returns an actual room base model by its ID. If no room is found, returns null.
        public Room? GetRoomModelById(int roomId)
        {
            Room? room = _repository.GetRooms().Where(x => x.RoomId == roomId).FirstOrDefault();
            if (room != null)
            {
                return room;
            }
            else
            {
                return null;
            }
        }

        // Finds and returns a room by its name using fuzzymatching as default. If no room is found, returns null.
        public RoomViewModel? GetRoomByName(string roomName, bool fuzzyMatching = true)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                return null;

            List<Room> rooms = _repository.GetRooms().ToList(); //Get the rooms first (for indexing purposes in the 
            List<string> roomNames = rooms.Select(room => room.Name).ToList();

            if (fuzzyMatching)
            {
                // Use FuzzySharp to find the best match
                var bestMatch = FuzzySharp.Process.ExtractOne(roomName, roomNames,
                    s => s,
                    ScorerCache.Get<PartialRatioScorer>());

                // You can adjust this threshold as needed
                if (bestMatch.Score >= 70)
                {
                    var index = bestMatch.Index;
                    var room = rooms[index];
                    return new RoomViewModel(room);
                }
                return null;
            }
            else
            {
                var room = rooms.FirstOrDefault(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
                return new RoomViewModel(room);
            }
        }

        // Finds and returns an actual room base model by its name using fuzzymatching as default. If no room is found, returns null.
        public Room? GetRoomModelByName(string roomName, bool fuzzyMatching = true)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                return null;

            List<Room> rooms = _repository.GetRooms().ToList(); //Get the rooms first (for indexing purposes in the 
            List<string> roomNames = rooms.Select(room => room.Name).ToList();

            if (fuzzyMatching)
            {
                // Use FuzzySharp to find the best match
                var bestMatch = FuzzySharp.Process.ExtractOne(roomName, roomNames,
                    s => s,
                    ScorerCache.Get<PartialRatioScorer>());

                // You can adjust this threshold as needed
                if (bestMatch.Score >= 70)
                {
                    var index = bestMatch.Index;
                    var room = rooms[index];
                    return room;
                }
                return null;
            }
            else
            {
                var room = rooms.FirstOrDefault(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
                return room;
            }
        }

        // Retrieves a list of rooms filtered by optional criteria: room type, capacity, and location.
        public List<RoomViewModel> GetRoomsByFilter(RoomType? roomType = null, int? capacity = null, Location? location = null)
        {
            // Start with all rooms from the repository
            var rooms = _repository.GetRooms().AsQueryable();

            // Apply filters conditionally based on the parameters
            if (roomType.HasValue)
            {
                //Converting the roomType to string to do proper string comparison
                rooms = rooms.Where(r => r.Type.Equals(roomType.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            if (capacity.HasValue)
            {
                rooms = rooms.Where(r => r.Capacity >= capacity.Value);
            }

            if (!string.IsNullOrEmpty(location?.ToString())) // Make sure to check if location is not null or empty
            {
                rooms = rooms.Where(r => r.Location.Equals(location.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            // Return the filtered list as a List<RoomViewModel>
            return rooms.Select(room => new RoomViewModel(room)).ToList();
        }

        //public List<RoomViewModel> GetRoomsByAmenities(string amenity, bool fuzzyMatching)
        //{

        //    List<Room> rooms = _repository.GetRooms().ToList(); //Get the rooms first (for indexing purposes in the 
        //    var filteredRooms = new List<Room>();

        //    if (fuzzyMatching)
        //    {

        //        foreach (var room in rooms)
        //        {
        //            List<string> roomAmenities = room.Amenities.Split(',').Select(a => a.Trim()).ToList();

        //            // Check if the specified amenity matches using FuzzySharp
        //            var bestMatch = FuzzySharp.Process.ExtractOne(
        //                amenity,
        //                roomAmenities,
        //                s => s,
        //                ScorerCache.Get<PartialRatioScorer>() // Make sure this is set correctly
        //            );

        //            // Add the room to the filtered list if the best match score is greater than or equal to 70
        //            if (bestMatch.Score >= 70)
        //            {
        //                filteredRooms.Add(room);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // Filter rooms that contain the specified amenity (case-insensitive)
        //        filteredRooms = rooms.Where(room =>
        //            room.Amenities.Split(',')
        //                .Select(a => a.Trim())
        //                .Contains(amenity, StringComparer.OrdinalIgnoreCase)).ToList();
        //    }

        //    return filteredRooms.Select(room => new RoomViewModel(room)).ToList(); // Materialize the query to a List<Room>
        //}

        //Rooms that are booked on a specific date
        public List<RoomViewModel> GetRoomsByDate(DateTime? date)
        {
            var rooms = _repository.GetRooms().AsQueryable();

            if (date.HasValue)
            {
                // Convert the parameter date to date only (without time)
                var dateOnly = date.Value.Date;

                // Filter rooms that have bookings on the specified date
                rooms = rooms.Where(r => r.Bookings.Any(b =>
                    b.BookedDate.HasValue &&
                    b.BookedDate.Value.Date == dateOnly
                ));
            }

            return rooms.Select(room => new RoomViewModel(room)).ToList();
        }

        // Alternative implementation if you want rooms that are NOT booked on that date
        public List<RoomViewModel> GetAvailableRoomsByDate(DateTime? date)
        {
            var rooms = _repository.GetRooms().AsQueryable();

            if (date.HasValue)
            {
                // Convert the parameter date to date only (without time)
                var dateOnly = date.Value.Date;

                // Filter rooms that don't have bookings on the specified date
                rooms = rooms.Where(r => !r.Bookings.Any(b =>
                    b.BookedDate.HasValue &&
                    b.BookedDate.Value.Date == dateOnly
                ));
            }

            return rooms.Select(room => new RoomViewModel(room)).ToList();
        }

        #endregion

        #region UPDATE (CRUD)

        public void UpdateRoomInfo(RoomViewModel model, int userId)
        {
            var room = new Room();
            try
            {
                _mapper.Map(model, room);
                room.UpdatedDate = DateTime.Now;
                room.UpdatedBy = userId;
                room.IsDeleted = false; //Since RoomViewModel doesn't have IsDeleted Property it won't be mapped ot the RoomModel (Base)
                _repository.UpdateRoomInfo(room);
            }
            catch (Exception)
            {
                throw; //rethrow error
            }
        }

        #endregion

        #region DELETE (CRUD)

        public void HardDeleteRoom(int roomId)
        {
            var room = _repository.GetRooms().Where(x => x.RoomId.Equals(roomId)).FirstOrDefault();
            if(room != null)
            {
                _repository.DeleteRoom(room);
            }
        }

        public void SoftDeleteRoom(int roomId)
        {
            var room = _repository.GetRooms().Where(x => x.RoomId.Equals(roomId)).FirstOrDefault();
            if (room != null)
            {
                room.IsDeleted = true;
                _repository.UpdateRoomInfo(room);
            }
        }

        #endregion

        #region Others
        public bool IsRoomAvailableDate(int roomId, DateTime dateTime)
        {
            Room? room = GetRoomModelById(roomId);
            if (room == null)
            {
                throw new ArgumentException($"RoomId {roomId} queried doesn't exist, please try again");
            }

            var bookingsOnDate = room.Bookings
                .Where(b => b.BookedDate?.Date == dateTime.Date);

            if (!bookingsOnDate.Any()) return true;

            // Room is available if all bookings for that date are either disapproved or cancelled
            return bookingsOnDate.All(b =>
                b.Status?.ToLower() != "approved");
        }

        //public List<string>? ListAmenitiesByRoomId(int roomId)
        //{
        //    Room? room = GetRoomModelById(roomId);
        //    return (room != null) ? room.Amenities.Split(',').Select(a => a.Trim()).ToList() : null;
        //}

        //public List<string>? ListAmenitiesByRoomName(string roomName, bool fuzzyMatching)
        //{
        //    Room? room = GetRoomModelByName(roomName, fuzzyMatching);
        //    if (room != null)
        //    {
        //        return ListAmenitiesByRoomId(room.RoomId);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public List<DateTime?>? GetRoomBookingDatesByRoomId(int roomId){
            Room? room = GetRoomModelById(roomId);
            return (room != null) ? room.Bookings.Select(s => s.BookedDate).ToList(): null;
        }

        public List<DateTime?>? GetRoomBookingDatesByRoomName(string roomName, bool fuzzyMatching)
        {
            Room? room = GetRoomModelByName(roomName, fuzzyMatching);
            if (room != null)
            {
                return GetRoomBookingDatesByRoomId(room.RoomId);
            }
            else
            {
                return null;
            }
        }

        public List<User>? GetBookedUsersByRoomId(int roomId)
        {
            Room? room = GetRoomModelById(roomId);
            return (room != null) ?room.Bookings.Select(b => b.User).Distinct().ToList(): null;
        }



        public List<User>? GetBookedUsersByRoomName(string roomName, bool fuzzyMatching)
        {
            Room? room = GetRoomModelByName(roomName, fuzzyMatching);
            if (room != null)
            {
                return GetBookedUsersByRoomId(room.RoomId);
            }
            else
            {
                return null;
            }
        }

        public List<string>? GetBookedUsersNameByRoomId(int roomId)
        {
            Room? room = GetRoomModelById(roomId);
            if (room == null) return null;

            return room.Bookings
                .Select(b => FormatUserName(b.User))
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .ToList();
        }

        public List<string>? GetBookedUsersNameByRoomName(string roomName, bool fuzzyMatching)
        {
            Room? room = GetRoomModelByName(roomName, fuzzyMatching);
            if (room != null)
            {
                return GetBookedUsersNameByRoomId(room.RoomId);
            }
            else
            {
                return null;
            }
        }

        string FormatUserName(User user)
        {
            string firstName = user.FirstName?.Trim() ?? "";
            string lastName = user.LastName?.Trim() ?? "";

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return "";

            return $"{firstName} {lastName}".Trim();
        }

        #endregion
    }
}
