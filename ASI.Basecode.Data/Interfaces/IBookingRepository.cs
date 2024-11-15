using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetBookings();
        void AddBooking(Booking booking);
        bool BookingExists(int bookingId);
        void UpdateBookingInfo(Booking booking);
        void DeleteBooking(Booking booking);
    }
}
