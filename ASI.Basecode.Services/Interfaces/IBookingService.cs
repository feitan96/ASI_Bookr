using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookingService
    {
        #nullable enable
        //CREATE
        int AddBooking(BookingViewModel model) ; //return boookingId after booking object is pushed to the database

        //READ
        List<BookingViewModel> GetBookings();
        List<BookingViewModel> GetBookingsByIds(List<int> bookingIds);
        BookingViewModel? GetBooking(int Id);
        BookingViewModel? GetBookingByIdNoTracking(int bookingID);

        //UPDATE
        void UpdateBookingInfo(BookingViewModel model, int userId);
        void UpdateBookingStatus(BookingViewModel model, int userId, string status);

        //DELETE
        //void SoftDeleteBooking(int Id);
        void HardDeleteBooking(int Id);

        //OTHERS
        List<BookingViewModel> GetConflictBookings(BookingViewModel model, int? roomId);
        List<RoomViewModel> GetAvailableRoomsForBooking(BookingViewModel model);
    }
}
