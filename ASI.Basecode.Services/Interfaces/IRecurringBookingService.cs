using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRecurringBookingService
    {
        #nullable enable
        //CREATE
        int AddRecurrentBooking(RecurringBookingViewModel model);

        //READ
        List<RecurringBookingViewModel> GetAllRecurrentBookings();
        RecurringBookingViewModel GetRecurrentBookingById(int recurrentId);
        List<RecurringBookingViewModel> GetRecurrentBookingsByBookingId(int bookingId);


        //UPDATE
        void UpdateImage(RecurringBookingViewModel model);

        //DELETE
        void DeleteRecurrentBookingById(int recurrentId);
        void DeleteRecurrentBookingByBookingId(int bookingId); //either on or more image deletion
        void DeleteRecurrentBooking(int bookingId, string day);
        //OTHERS
    }
}
