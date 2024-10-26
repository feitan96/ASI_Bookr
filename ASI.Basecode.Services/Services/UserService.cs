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
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == email &&
                                                     x.Password == passwordKey && 
                                                     x.IsDeleted == false).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public List<UserViewModel> GetAllUser()
        {
            var users = _repository.GetUsers().Where(x => x.IsDeleted == false).Select(s => new UserViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber,
            }).ToList();

            return users;
        }
        public UserViewModel GetUser(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(Id)).Select(s => new UserViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber,
                Password = PasswordManager.DecryptPassword(s.Password)
            }).FirstOrDefault();

            return user;
        }

        public void AddUser(UserViewModel model, int userId)
        {
            if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            var user = new User();
            _mapper.Map(model, user);
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;
            user.IsDeleted = false;
            user.CreatedBy = userId;
            user.UpdatedBy = userId;
            user.UserId = "None"; //remove once UserId in DB is remove

            _repository.AddUser(user);
        }
        public void UpdateUser(UserViewModel model, int userId)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if (model.Email != user.Email)
            {
                if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }

            _mapper.Map(model, user);
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = userId;

            _repository.UpdateUser(user);
        }
        public void SoftDelete(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(Id)).FirstOrDefault();
            if (user != null)
            {
                user.IsDeleted = true;
                _repository.UpdateUser(user);
            }
        }
        public void HardDelete(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(Id)).FirstOrDefault();
            if(user != null)
            {
                _repository.DeleteUser(user);
            }
        }
    }
}
