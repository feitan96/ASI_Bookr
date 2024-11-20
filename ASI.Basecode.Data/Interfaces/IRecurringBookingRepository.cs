using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRecurringBookingRepository
    {
        IQueryable<RecurringBooking> GetRecurringBookings();
        void AddRecurringBooking(RecurringBooking booking);
        bool RecurringBookingExists(int bookingId);
        void UpdateRecurringBookingInfo(RecurringBooking booking);
        void DeleteRecurringBooking(RecurringBooking booking);
    }
}
