using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RecurringBookingService : IRecurringBookingService
    {
        private readonly IRecurringBookingRepository _repository;
        private readonly IMapper _mapper;

        public RecurringBookingService(IRecurringBookingRepository recurringBookingRepository, IMapper mapper)
        {
            _repository = recurringBookingRepository;
            _mapper = mapper;
        }

        #region CREATE (CRUD)

        // Adds a new recurring booking if it doesn't already exist, otherwise throws an exception.
        public int AddRecurrentBooking(RecurringBookingViewModel model)
        {
            var recurringBooking = new RecurringBooking();
            if (!_repository.RecurringBookingExists(model.RecurringId))
            {
                _mapper.Map(model, recurringBooking);
                _repository.AddRecurringBooking(recurringBooking);
                return recurringBooking.RecurringId;
            }
            else
            {
                throw new InvalidDataException("Recurring booking already exists.");
            }
        }

        #endregion

        #region READ (CRUD)

        // Retrieves all recurring bookings
        public List<RecurringBookingViewModel> GetAllRecurrentBookings()
        {
            var recurringBookings = _repository.GetRecurringBookings()
                .Select(rb => _mapper.Map<RecurringBookingViewModel>(rb))
                .ToList();
            return recurringBookings;
        }

        // Retrieves a specific recurring booking by its ID
        public RecurringBookingViewModel GetRecurrentBookingById(int recurrentId)
        {
            var recurringBooking = _repository.GetRecurringBookings()
                .FirstOrDefault(rb => rb.RecurringId == recurrentId);
            return recurringBooking != null ? _mapper.Map<RecurringBookingViewModel>(recurringBooking) : null;
        }

        // Retrieves recurring bookings associated with a specific booking ID
        public List<RecurringBookingViewModel> GetRecurrentBookingsByBookingId(int bookingId)
        {
            var recurringBookings = _repository.GetRecurringBookings()
                .Where(rb => rb.BookingId == bookingId)
                .Select(rb => _mapper.Map<RecurringBookingViewModel>(rb))
                .ToList();
            return recurringBookings;
        }

        #endregion

        #region UPDATE (CRUD)

        // Updates an existing recurring booking
        public void UpdateImage(RecurringBookingViewModel model)
        {
            var recurringBooking = new RecurringBooking();
            try
            {
                _mapper.Map(model, recurringBooking);
                _repository.UpdateRecurringBookingInfo(recurringBooking);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the recurring booking.", ex);
            }
        }

        #endregion

        #region DELETE (CRUD)

        // Deletes a recurring booking by its ID
        public void DeleteRecurrentBookingById(int recurrentId)
        {
            var recurringBooking = _repository.GetRecurringBookings()
                .FirstOrDefault(rb => rb.RecurringId == recurrentId);

            if (recurringBooking != null)
            {
                _repository.DeleteRecurringBooking(recurringBooking);
            }
        }

        // Deletes recurring bookings by the associated booking ID
        public void DeleteRecurrentBookingByBookingId(int bookingId)
        {
            var recurringBookings = _repository.GetRecurringBookings()
                .Where(rb => rb.BookingId == bookingId)
                .ToList();

            foreach (var recurringBooking in recurringBookings)
            {
                _repository.DeleteRecurringBooking(recurringBooking);
            }
        }

        public void DeleteRecurrentBooking(int bookingId, string day)
        {
            var recurringBooking = _repository.GetRecurringBookings()
                .Where(rb => (rb.BookingId == bookingId && rb.Day == day)).FirstOrDefault();

            if (recurringBooking != null)
            {
                _repository.DeleteRecurringBooking(recurringBooking);
            }

        }

        #endregion
    }
}
