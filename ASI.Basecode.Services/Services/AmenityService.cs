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
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _repository;
        private readonly IMapper _mapper;

        public AmenityService(IAmenityRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // CREATE
        public void AddAmenity(AmenityViewModel model)
        {
            var amenity = new Amenity();
            // Check if the amenity already exists
            if (!_repository.AmenityExists(model.AmenityId))
            {
                // Map the view model to the entity using _mapper
                _mapper.Map(model, amenity);
                // Add the amenity to the repository
                _repository.AddAmenity(amenity);
            }
            else
            {
                // Throw an exception if the amenity already exists
                throw new InvalidDataException("The amenity already exists.");
            }
        }

        // READ
        public AmenityViewModel GetAmenity(int id)
        {
            var amenity = _repository.GetAmenities().FirstOrDefault(x => x.AmenityId == id);
            if (amenity == null) return null;

            return new AmenityViewModel(amenity);
        }

        // UPDATE
        public void UpdateAmenityInfo(AmenityViewModel model)
        {
            var amenity = new Amenity();
            try
            {
                _mapper.Map(model, amenity);
                _repository.UpdateAmenity(amenity);
            }
            catch (Exception)
            {
                throw; //rethrow error
            }
        }

        // DELETE
        public void DeleteAmenity(int id)
        {
            var amenity = _repository.GetAmenities().FirstOrDefault(x => x.AmenityId == id);
            if (amenity == null) return;

            _repository.DeleteAmenity(amenity);
        }
    }
}
