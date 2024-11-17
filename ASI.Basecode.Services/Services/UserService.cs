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
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IAdminRepository adminRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _adminRepository = adminRepository;
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

        public PagedResult<UserViewModel> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = _repository.GetUsers()
                           .Where(x => (bool)!x.IsDeleted);

            var totalRecords = users.Count();
            var paginatedUsers = users.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .Select(s => new UserViewModel
                                      {
                                          Id = s.Id,
                                          FirstName = s.FirstName,
                                          LastName = s.LastName,
                                          Email = s.Email,
                                          Role = s.Role,
                                          PhoneNumber = s.PhoneNumber,
                                      })
                                      .ToList();

            return new PagedResult<UserViewModel>
            {
                Items = paginatedUsers,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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
                Password = s.Password
            }).FirstOrDefault();

            return user;
        }

        public void AddUser(UserViewModel model, int userId)
        {
            if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);

            var user = new User();
            _mapper.Map(model, user);
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.CreatedDate = user.UpdatedDate = DateTime.Now;
            user.CreatedBy = user.UpdatedBy = userId;
            user.IsDeleted = user.IsDarkMode = false;
            user.AllowNotifications = true;
            user.DefaultBookDuration = 3;
            user.UserId = "None"; //remove once UserId in DB is remove

            _repository.AddUser(user);

            if(user.Role == "Admin") _adminRepository.AddAdmin(new Admin { UserId = user.Id });
        }
        public void UpdateUser(UserViewModel model, int userId)
        {
            var user = _repository.GetUsers().Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if (model.Email != user.Email)
            {
                if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }

            if (model.Role == "Admin" && user.Role != "Admin")
            {
                _adminRepository.AddAdmin(new Admin { UserId = user.Id });
            }
            else if (model.Role != "Admin" && user.Role == "Admin")
            {
                var admin = _adminRepository.GetAdmins().Where(x => x.UserId.Equals(user.Id)).FirstOrDefault();
                if (admin != null) _adminRepository.RemoveAdmin(admin);
            }
            if(model.Password != user.Password) model.Password = PasswordManager.EncryptPassword(model.Password);
            _mapper.Map(model, user);
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
    }
}
