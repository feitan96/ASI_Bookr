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

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.UserId == userId &&
                                                     x.Password == passwordKey && 
                                                     x.IsDeleted == false).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public List<UserViewModel> GetAllUser()
        {
            var users = _repository.GetUsers().Select(s => new UserViewModel
            {
                Id = s.Id,
                UserId = s.UserId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber,
                IsDeleted = s.IsDeleted == true
            }).ToList();

            return users;
        }
        public UserViewModel GetUser(int Id)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(Id)).Select(s => new UserViewModel
            {
                Id = s.Id,
                UserId = s.UserId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber,
                Password = PasswordManager.DecryptPassword(s.Password)
            }).FirstOrDefault();

            return user;
        }

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            try
            {
                if (!_repository.UserExists(model.UserId))
                {
                    _mapper.Map(model, user);
                    user.Password = PasswordManager.EncryptPassword(model.Password);
                    user.CreatedDate = DateTime.Now;
                    user.UpdatedDate = DateTime.Now;
                    user.IsDeleted = false;
                    //user.CreatedBy = System.Environment.UserName;
                    //user.UpdatedBy = System.Environment.UserName;

                    _repository.AddUser(user);
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }
        public void UpdateUser(UserViewModel model, string userId)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if(user != null)
            {
                _mapper.Map(model, user);
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.UpdatedDate = DateTime.Now;
                //user.UpdatedBy = userId;

                _repository.UpdateUser(user);
            }
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
