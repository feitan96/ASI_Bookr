using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class BookingService : IBookingService
    {
#nullable enable

        private readonly IServiceProvider _serviceProvider;
        private readonly IBookingRepository _repository;
        private readonly IUserService _userservice;
        private readonly IEmailService _emailservice;
        private readonly IRoomService _roomservice;
        private readonly IRecurringBookingService _recurringbookingservice;
        private readonly IMapper _mapper;

        public BookingService(IServiceProvider serviceProvider, IUserService userService, IEmailService emailService, IBookingRepository repository, IRoomService roomService, IMapper mapper, IRecurringBookingService recurringBookingService)
        {
            _mapper = mapper;
            _userservice = userService;
            _roomservice = roomService;
            _emailservice = emailService;
            _repository = repository;
            _recurringbookingservice = recurringBookingService;
            _serviceProvider = serviceProvider;
        }

        #region Create (CRUD)

        //CREATE
        public int AddBooking(BookingViewModel model) //return boookingId after booking object is pushed to the database
        {
            var booking = new Booking();
            if (!_repository.BookingExists(model.BookingId))
            {
                _mapper.Map(model, booking);
                booking.User = null; ////User object should be nul by default, since we only need the userId during booking
                booking.Room = null; //Room object should be nul by default, since we only need the roomId during booking
                booking.CreatedDate = DateTime.Now;
                booking.UpdatedDate = DateTime.Now;
                booking.Status = model.Status ?? "Pending"; //if model is null then pass "Pending" as status by default
                if (!booking.IsRecurring)
                {
                    //If booking happens only once, then the bookingEndDate and StartDate must be the same else, accept user's input of booking End Date
                    booking.BookingEndDate = booking.BookingStartDate;
                }
                _repository.AddBooking(booking);

                //Get bookingId after successful booking for setting recurring table
                var bookingId = booking.BookingId;
                //Populate the recurring table if user setup IsRecurring value to true
                if (bookingId != null && booking.IsRecurring)
                {
                    model.SelectedDaysList.ForEach(
                        day =>
                        {
                            var recurringBooking = new RecurringBookingViewModel(bookingId, day);
                            _recurringbookingservice.AddRecurrentBooking(recurringBooking);
                        }
                        );
                }

                return booking.BookingId;
            }
            else
            {
                // Throw an exception if the booking already made
                throw new InvalidDataException("This booking already made.");
            }

        }
        
        #endregion


        #region Read (CRUD)
        //READ
        public List<BookingViewModel> GetBookings()
        {
            //var bookings = _repository.GetBookings().ToList();
            //var bookingsModel = new List<BookingViewModel>();

            //foreach(var booking in bookings)
            //{
            //    var bookingModel = new BookingViewModel();
            //    _mapper.Map(booking, bookingModel);
            //    bookingsModel.Add(bookingModel);
            //}

            //return bookingsModel;
            return _repository.GetBookings()
                  .Select(booking => _mapper.Map<BookingViewModel>(booking))
                  .ToList();
        }

        public List<BookingViewModel> GetBookingsByIds(List<int> bookingIds)
        {
            if (bookingIds == null || !bookingIds.Any())
                return new List<BookingViewModel>();

            return _repository.GetBookings().Where(booking => bookingIds.Contains(booking.BookingId))
              .Select(booking => _mapper.Map<BookingViewModel>(booking))
              .ToList();
        }

        public BookingViewModel? GetBooking(int Id)
        {
            var booking = _repository.GetBookings().Where(booking => booking.BookingId == Id).FirstOrDefault();
            if (booking != null)
            {
                return _mapper.Map<BookingViewModel>(booking);
            }
            else
            {
                return null;
            }
        }

        public BookingViewModel? GetBookingByIdNoTracking(int bookingID)
        {
            var booking = _repository.GetBookingsNoTracking().Where(booking => booking.BookingId == bookingID).FirstOrDefault();
            if (booking != null)
            {
                return _mapper.Map<BookingViewModel>(booking);
            }
            else
            {
                return null;
            }

        }

        #endregion

        #region Update (CRUD)

        //UPDATE
        public void UpdateBookingInfo(BookingViewModel model, int userId)
        {
            var booking = new Booking();
            try
            {
                _mapper.Map(model, booking);
                booking.User = null; ////User object should be nul by default, since we only need the userId during update of booking
                booking.Room = null; //Room object should be nul by default, since we only need the roomId during updating of booking
                booking.UpdatedDate = DateTime.Now;
                booking.UpdatedBy = userId;
                if (!booking.IsRecurring)
                {
                    //If booking happens only once, then the bookingEndDate and StartDate must be the same else, accept user's input of booking End Date
                    booking.BookingEndDate = booking.BookingStartDate;

                    //If user decided to make the booking non-recurring then (if there are previous selected Days for recurring) then remove it, to minimize or avoid logic conflict when tracking conflicts with other bookings
                    _recurringbookingservice.DeleteRecurrentBookingByBookingId(booking.BookingId);
                }
                else
                {
                    if (model.Status == "Pending") //Editing and updating of the selected Days for recurring booking must only be done with pending bookings (not on approved/disapproved/cancelled ones)
                    {
                        var userSelectedDays = model.SelectedDaysList; //from the ViewModel (selected by the user)
                        var currentSelectedDays = _recurringbookingservice.GetRecurrentBookingsByBookingId(booking.BookingId).Select(x => x.Day).ToList();
                        var selectedDaysToRemove = currentSelectedDays.Except(userSelectedDays).ToList();
                        var selectedDaysToAdd = userSelectedDays.Except(currentSelectedDays).ToList();

                        foreach (var day in selectedDaysToRemove)
                        {
                            _recurringbookingservice.DeleteRecurrentBooking(booking.BookingId, day);
                        }

                        foreach (var day in selectedDaysToAdd)
                        {
                            var recurrentBooking = new RecurringBookingViewModel(booking.BookingId, day);
                            _recurringbookingservice.AddRecurrentBooking(recurrentBooking);
                        }

                    }
                }
                _repository.UpdateBookingInfo(booking);
            }
            catch (Exception)
            {
                throw; //rethrow error
            }
        }

        public void UpdateBookingStatus(BookingViewModel model, int userId, string status)
        {
            var booking = new Booking();
            try
            {
                _mapper.Map(model, booking);
                booking.User = null;
                booking.Room = null;
                booking.UpdatedDate = DateTime.Now;
                booking.UpdatedBy = userId;
                booking.Status = status;
                if(status == "Approved" || status == "Disapproved")
                {
                    booking.ApproveDisapproveBy = userId;
                }

                _repository.UpdateBookingInfo(booking);

                // Get user email from the booking
                var userDetails = _userservice.GetUser(model.UserId);
                if (userDetails != null)
                {
                    // Prepare email content
                    var subject = $"Bookr: Your LRAC Booking has been {status}";
                    var body = $@"Your booking with details:
Title: {model.Title}
Room: {model.RoomName}
Date: {model.BookingStartDate.ToString("MMMM dd, yyyy")}
Check-in Time: {model.CheckInTime.ToString(@"hh\:mm")}
Check-out Time: {model.CheckOutTime.ToString(@"hh\:mm")}

Has been {status}. Thank you, Teknoy!";

                    // Send email
                    _emailservice.SendEmail(userDetails.Email, subject, body);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Delete (CRUD)

        //DELETE
        public void HardDeleteBooking(int Id)
        {
            var booking = _repository.GetBookings().Where(booking => booking.BookingId == Id).FirstOrDefault();
            if (booking != null)
            {
                _repository.DeleteBooking(booking);
            }
        }
        #endregion

        #region OTHERS
        public List<BookingViewModel> GetConflictBookings(BookingViewModel model, int? roomId)
        {
            //Get Conflict Bookings: STEPS
            //1) Get the Bookings where the roomId is the same as the one being booked by the user
            //2) Get the Bookings where the booking status is either pending or approved
            //3) Check Date Conflict, Time Conflict, and Recurring Conflicts

            // Convert the BookingViewModel to a Booking entity using AutoMapper
            var bookingToCompare = _mapper.Map<Booking>(model);

            // Retrieve all bookings for the same RoomId
            roomId ??= model.RoomId;
            var bookings = GetApprovedAndPendingBookingsByRoomId(roomId.Value);

            var conflictingBookings = new List<Booking>();

            foreach (var existingBooking in bookings)
            {
                // Check if dates overlap
                bool isDateConflict = bookingToCompare.BookingStartDate <= existingBooking.BookingEndDate &&
                                      bookingToCompare.BookingEndDate >= existingBooking.BookingStartDate;

                // Check if times overlap
                bool isTimeConflict = bookingToCompare.CheckInTime < existingBooking.CheckOutTime &&
                                      bookingToCompare.CheckOutTime > existingBooking.CheckInTime;

                // Handle recurring vs. non-recurring conflict
                bool isRecurringConflict = bookingToCompare.IsRecurring || existingBooking.IsRecurring
                    ? CheckRecurringConflict(model, existingBooking) //Since bookingToCompare has no yet accessible RecurringBookings Mapping, we need to get the SelectedDaysList
                    : true; // Non-recurring bookings always conflict if dates/times overlap

                // Add conflicting bookings to the list if any conflict condition is met
                if (isDateConflict && isTimeConflict && isRecurringConflict)
                {
                    conflictingBookings.Add(existingBooking);
                }
            }


            // Convert the conflicting bookings back to BookingViewModel
            return _mapper.Map<List<BookingViewModel>>(conflictingBookings);
        }

        private List<Booking> GetApprovedAndPendingBookingsByRoomId(int roomId)
        {  //Reusability for conflicts and suggestions handling
            return _repository.GetBookings()
                .Where(booking => booking.RoomId == roomId && (booking.Status == "Approved" || booking.Status == "Pending"))
                .ToList();
        }

        // Helper to check recurring conflict
        private bool CheckRecurringConflict(BookingViewModel booking1, Booking booking2)
        {
            if (booking1.IsRecurring && booking2.IsRecurring)
            {
                // Both bookings are recurring, check for day overlap
                var days1 = booking1.SelectedDaysList.Select(day => day.ToString()).ToList();
                var days2 = booking2.RecurringBookings.Select(rb => rb.Day).ToList();
                return days1.Intersect(days2).Any();
            }

            // One booking is recurring, check if non-recurring booking overlaps with recurring days
            object recurringBooking = booking1.IsRecurring ? booking1 : booking2;
            object nonRecurringBooking = booking1.IsRecurring ? booking2 : booking1;

            // Check if the non-recurring booking's day conflicts with the recurring days
            // Check if the non-recurring booking's day conflicts with the recurring days
            var nonRecurringDay = nonRecurringBooking switch
            {
                BookingViewModel bookingViewModel => bookingViewModel.BookingStartDate.DayOfWeek.ToString(),
                Booking bookingEntity => bookingEntity.BookingStartDate.DayOfWeek.ToString(),
                _ => string.Empty
            };


            return recurringBooking switch
            {
                BookingViewModel bookingViewModel => bookingViewModel.SelectedDaysList.Any(day => day.ToString() == nonRecurringDay),
                Booking bookingEntity => bookingEntity.RecurringBookings.Any(rb => rb.Day == nonRecurringDay),
                _ => false
            };
        }


        //Non-thread safe method
        //public List<RoomViewModel> GetAvailableRoomsForBooking(BookingViewModel model)
        //{
        //    var allRooms = _roomservice.GetRooms();
        //    var batchSize = Math.Min(Environment.ProcessorCount * 2, allRooms.Count);

        //    return allRooms
        //        .AsParallel()
        //        .WithDegreeOfParallelism(batchSize)
        //        .Where(room => !GetConflictBookings(model, room.RoomId).Any())
        //        .ToList();
        //}

        public List<RoomViewModel> GetAvailableRoomsForBooking(BookingViewModel model)
        {
            // Fetch all rooms
            var allRooms = _roomservice.GetRooms();
            var availableRooms = new ConcurrentBag<RoomViewModel>(); // Thread-safe collection

            Parallel.ForEach(allRooms, room =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var conflictingBookings = bookingService.GetConflictBookings(model, room.RoomId);

                    if (!conflictingBookings.Any())
                    {
                        availableRooms.Add(room);
                    }
                }
            });

            return availableRooms.ToList();
        }
        #endregion



    }
}
