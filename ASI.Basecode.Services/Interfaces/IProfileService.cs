using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IProfileService
    {
        ProfileViewModel GetUser(int Id);
        void UpdateUser(ProfileViewModel model, int userId);
    }
}
