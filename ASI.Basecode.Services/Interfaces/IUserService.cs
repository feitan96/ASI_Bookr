﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        List<UserViewModel> GetAllUser();
        UserViewModel GetUser(int Id);
        void AddUser(UserViewModel model, int userId);
        void UpdateUser(UserViewModel model, int userId);
        void SoftDelete(int Id);
        void HardDelete(int Id);
    }
}
