using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public ProfileService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public ProfileViewModel GetUser(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(Id)).Select(s => new ProfileViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                AllowNotifications = s.Equals(true),
                IsDarkMode = s.Equals(true),
                DefaultBookDuration = s.DefaultBookDuration,
            }).FirstOrDefault();

            return user;
        }
        public void UpdateUser(ProfileViewModel model, int userId)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if (model.Email != user.Email)
            {
                if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }

            _mapper.Map(model, user);
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = userId;

            _repository.UpdateUser(user);
        }
    }
}
