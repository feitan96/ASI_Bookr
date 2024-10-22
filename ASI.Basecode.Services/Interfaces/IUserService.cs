using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        List<UserViewModel> GetAllUser();
        UserViewModel GetUser(int Id);
        void AddUser(UserViewModel model);
        void UpdateUser(UserViewModel model, string userId);
        void SoftDelete(int Id);
        void HardDelete(int Id);
    }
}
