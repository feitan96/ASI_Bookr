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
using System.Text.RegularExpressions;
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

        public UserPaginationViewModel GetPagedUsers(int page = 1, int pageSize = 1, string searchName = "", string filterRole = "")
        {
            var query = _repository.GetUsers().Where(x => x.IsDeleted == false)
                .Select(s => new UserViewModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Role = s.Role,
                    PhoneNumber = s.PhoneNumber,
                    CreatedDate = s.CreatedDate,
                });

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(u =>
                    (u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filterRole))
            {
                query = query.Where(u => u.Role.ToLower() == filterRole.ToLower());
            }

            // Pagination
            var totalUsers = query.Count();
            var users = query
                .OrderBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new UserPaginationViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                PageSize = pageSize,
                SearchName = searchName,
                FilterRole = filterRole
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

        #region Forgot Password
        public string GeneratePasswordResetToken(string email)
        {
            var user = _repository.GetUsers().Where(x => x.Email.Equals(email)).FirstOrDefault();
            if (user == null) return null;

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.ResetTokenExpiry = DateTime.Now.AddHours(1);
            _repository.UpdateUser(user);

            return token;
        }

        public Status ResetPassword(ResetPasswordModel model)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.PasswordResetToken == model.Token && u.ResetTokenExpiry > DateTime.Now);
            if (user == null) return Status.Error;

            user.Password = PasswordManager.EncryptPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            _repository.UpdateUser(user);

            return Status.Success;
        }

        public ChangePassToken IsTokenValid(string token)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.PasswordResetToken == token && u.ResetTokenExpiry > DateTime.Now);
            if (user == null) return ChangePassToken.Invalid;

            return ChangePassToken.Valid;
        }
        #endregion

        #region User CRUD
        public void AddUser(UserViewModel model, int userId)
        {
            if (_repository.UserExists(model.Email)) throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            if (!IsValidPassword(model.Password)) throw new InvalidDataException(Resources.Messages.Errors.PasswordError);
            var user = new User();
            _mapper.Map(model, user);
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.CreatedDate = DateTime.Now;
            user.CreatedBy = userId;
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

            if (model.Password != user.Password) user.Password = PasswordManager.EncryptPassword(model.Password);

            _mapper.Map(model, user);

            //var password = PasswordManager.DecryptPassword(user.Password); //No editing of password
            //if (!IsValidPassword(password)) throw new InvalidDataException(Resources.Messages.Errors.PasswordError);

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
        #endregion

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
                CreatedDate = s.CreatedDate,
            }).ToList();

            return users;
        }
        public void UpdateUserRole(int userId, string newRole)
        {
            var user = _repository.GetUsers().FirstOrDefault(x => x.Id == userId);
            if (user == null) throw new ArgumentException("User not found.");

            // Update Admin repository if role is changing
            if (newRole == "Admin" && user.Role != "Admin")
            {
                _adminRepository.AddAdmin(new Admin { UserId = user.Id });
            }
            else if (newRole != "Admin" && user.Role == "Admin")
            {
                var admin = _adminRepository.GetAdmins().FirstOrDefault(a => a.UserId == user.Id);
                if (admin != null) _adminRepository.RemoveAdmin(admin);
            }

            // Update the role and save changes
            user.Role = newRole;
            user.UpdatedDate = DateTime.Now;
            _repository.UpdateUser(user);
        }
        public static bool IsValidPassword(string password)
        {
            // Password must have at least:
            // - One uppercase letter
            // - One digit
            // - One special character
            // - Minimum of 8 characters
            //var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            var passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
