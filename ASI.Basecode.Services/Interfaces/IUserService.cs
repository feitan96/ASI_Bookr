using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        UserViewModel GetUser(int Id);
        void AddUser(UserViewModel model, int userId);
        void UpdateUser(UserViewModel model, int userId);
        void SoftDelete(int Id);
        void HardDelete(int Id);
        PagedResult<UserViewModel> GetAllUsers(int pageNumber, int pageSize);
        string GeneratePasswordResetToken(string email);
        Status ResetPassword(ResetPasswordModel model);
        ChangePassToken IsTokenValid(string token);
        List<UserViewModel> GetAllUser();
        void UpdateUserRole(int userId, string newRole);

    }
}
