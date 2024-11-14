using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
        public IQueryable<Admin> GetAdmins()
        {
            return this.GetDbSet<Admin>();
        }
        public void AddAdmin(Admin admin)
        {
            this.GetDbSet<Admin>().Add(admin);
            UnitOfWork.SaveChanges();
        }

        public void RemoveAdmin(Admin admin)
        {
            this.GetDbSet<Admin>().Remove(admin);
            UnitOfWork.SaveChanges();
        }
    }
}
