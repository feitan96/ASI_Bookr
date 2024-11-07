using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RoomAmenityService : IRoomAmenityService
    {
        private readonly IAmenityService _amenityService;
        private readonly IRoomAmenityRepository _repository;
        private readonly IMapper _mapper;

        public RoomAmenityService(IAmenityService amenityService, IRoomAmenityRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _amenityService = amenityService;
        }

        // CREATE
        public void AddRoomAmenity(RoomAmenityViewModel model)
        {
            var roomAmenity = new RoomAmenity();
            if (!_repository.RoomAmenityExists(model.RoomId, model.AmenityId))
            {
                _mapper.Map(model, roomAmenity);
                _repository.AddRoomAmenity(roomAmenity);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.RoomAmenityExists);
            }
        }

        public void AddRoomAmenity(int roomId, int amenityId)
        {
            var roomAmenity = new RoomAmenity();
            if (!_repository.RoomAmenityExists(roomId, amenityId))
            {
                roomAmenity.RoomId = roomId;
                roomAmenity.AmenityId = amenityId;
                _repository.AddRoomAmenity(roomAmenity);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.RoomAmenityExists);
            }
        }

        // READ
        public RoomAmenityViewModel GetRoomAmenity(int id)
        {
            var roomAmenity = _repository.GetRoomAmenities().Where(x => x.Id == id).FirstOrDefault();
            var roomAmenityModel = new RoomAmenityViewModel(roomAmenity);
            roomAmenityModel.Amenity = _amenityService.GetAmenity(roomAmenity.AmenityId);

            if (roomAmenity == null) return null;

            return roomAmenityModel;
        }

        public RoomAmenityViewModel GetRoomAmenity(int roomId, int amenityId)
        {
            var roomAmenity = _repository
                .GetRoomAmenities().Where(x => x.RoomId == roomId && x.AmenityId == amenityId).FirstOrDefault();
            var roomAmenityModel = new RoomAmenityViewModel(roomAmenity);
            roomAmenityModel.Amenity = _amenityService.GetAmenity(roomAmenity.AmenityId);

            if (roomAmenity == null) return null;

            return roomAmenityModel;
        }

        public List<RoomAmenityViewModel> GetRoomAmenities(int roomId)
        {
            var roomAmenities = _repository
                .GetRoomAmenities()
                .Where(x => x.RoomId == roomId)
                .Select(x => new RoomAmenityViewModel(x))
                .ToList();

            roomAmenities.ForEach(
                x =>
                    {
                        x.Amenity = _amenityService.GetAmenity(x.AmenityId);
                    }
                );

            return roomAmenities;
        }

        // UPDATE
        public void UpdateRoomAmenityInfo(RoomAmenityViewModel model)
        {
            var roomAmenity = new RoomAmenity();
            try
            {
                _mapper.Map(model, roomAmenity);
                _repository.UpdateRoomAmenity(roomAmenity);
            }
            catch (Exception)
            {
                throw; //rethrow error
            }
        }

        // DELETE
        public void DeleteRoomAmenity(int id)
        {
            var roomAmenity = _repository.GetRoomAmenities().Where(x => x.Id == id).FirstOrDefault();
            if (roomAmenity != null)
            {
                _repository.DeleteRoomAmenity(roomAmenity);
            }

        }

        public void DeleteRoomAmenity(int roomId, int amenityId)
        {
            var roomAmenity = _repository
                .GetRoomAmenities().Where(x => x.RoomId == roomId && x.AmenityId == amenityId)
                .FirstOrDefault();

            if (roomAmenity == null)
            {
                _repository.DeleteRoomAmenity(roomAmenity);
            }

        }
    }
}
