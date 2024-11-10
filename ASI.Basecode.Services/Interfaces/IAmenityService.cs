using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAmenityService
    {

        #nullable enable
        //CREATE
        void AddAmenity(AmenityViewModel model);

        //READ
        List<AmenityViewModel> GetAmenities();
        AmenityViewModel GetAmenity(int Id);

        //UPDATE
        void UpdateAmenityInfo(AmenityViewModel model);

        //DELETE
        void DeleteAmenity(int id);

        //OTHERS


    }
}
