using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        public ProfileService(IUserRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public ProfileViewModel GetUser(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id == Id && !(bool)x.IsDeleted).Select(s => new ProfileViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                AllowNotifications = (bool)s.AllowNotifications,
                IsDarkMode = (bool)s.IsDarkMode,
                DefaultBookDuration = s.DefaultBookDuration,
            }).FirstOrDefault();

            return user;
        }
        public void UpdateUser(ProfileViewModel model, int userId)
        {
            var user = _repository.GetUsers().Where(x => x.Id == model.Id && !(bool)x.IsDeleted).FirstOrDefault();

            // Optional check to ensure unique email if updating email
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
