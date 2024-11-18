using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookingRepository: BaseRepository, IBookingRepository
    {
        public BookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Booking> GetBookings()
        {
            var bookings = this.GetDbSet<Booking>().Include(booking => booking.Room).ThenInclude(room => room.RoomAmenities).ThenInclude(roomAmenity => roomAmenity.Amenity)
                .Include(booking => booking.User);

            return bookings;
        }

        public bool BookingExists(int bookingId)
        {
            return this.GetDbSet<Booking>().Any(booking => booking.BookingId == bookingId);
        }

        public void AddBooking(Booking booking)
        {
            try
            {
                this.GetDbSet<Booking>().Add(booking);
                UnitOfWork.SaveChanges();
                Console.WriteLine("Booking added successfully.");
            }
            catch (Exception ex)
            {
                // Log the error message and stack trace
                Console.WriteLine($"Error adding booking: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, rethrow the exception if you want it to propagate
                throw;
            }
        }

        public void UpdateBookingInfo(Booking booking)
        {
            try
            {
                this.GetDbSet<Booking>().Update(booking);
                UnitOfWork.SaveChanges();
                Console.WriteLine("Booking updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the error message and stack trace
                Console.WriteLine($"Error adding booking: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally, rethrow the exception if you want it to propagate
                throw;
            }
        }
        public void DeleteBooking(Booking booking)
        {
            this.GetDbSet<Booking>().Remove(booking);
            UnitOfWork.SaveChanges();
        }
    }
}
