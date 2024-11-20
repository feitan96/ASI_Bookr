using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class RecurringBookingRepository : BaseRepository, IRecurringBookingRepository
    {
        public RecurringBookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        // Get all recurring bookings
        public IQueryable<RecurringBooking> GetRecurringBookings()
        {
            return this.GetDbSet<RecurringBooking>();
        }

        // Check if a recurring booking exists by ID
        public bool RecurringBookingExists(int bookingId)
        {
            return this.GetDbSet<RecurringBooking>().Any(x => x.RecurringId == bookingId);
        }

        // Add a new recurring booking
        public void AddRecurringBooking(RecurringBooking recurringBooking)
        {
            this.GetDbSet<RecurringBooking>().Add(recurringBooking);
            UnitOfWork.SaveChanges();
        }

        // Update an existing recurring booking
        public void UpdateRecurringBookingInfo(RecurringBooking recurringBooking)
        {
            this.GetDbSet<RecurringBooking>().Update(recurringBooking);
            UnitOfWork.SaveChanges();
        }

        // Delete a recurring booking
        public void DeleteRecurringBooking(RecurringBooking recurringBooking)
        {
            this.GetDbSet<RecurringBooking>().Remove(recurringBooking);
            UnitOfWork.SaveChanges();
        }
    }
}
