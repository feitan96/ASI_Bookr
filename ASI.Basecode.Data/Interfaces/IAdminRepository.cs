using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAdminRepository
    {
        IQueryable<Admin> GetAdmins();
        void AddAdmin(Admin admin);
        void RemoveAdmin(Admin admin);
    }
}
