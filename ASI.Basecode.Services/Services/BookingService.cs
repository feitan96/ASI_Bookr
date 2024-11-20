using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookingService : IBookingService
    {
        #nullable enable

        private readonly IBookingRepository _repository;
        private readonly IUserService _userservice;
        private readonly IRoomService _roomservice;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repository,  IUserService userService,IRoomService roomService, IEmailService emailService, IMapper mapper)
        {
            _mapper = mapper;
            _userservice = userService;
            _roomservice = roomService;
            _emailService = emailService;
            _repository = repository;
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
                _repository.AddBooking(booking);
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
            var bookings = _repository.GetBookings().ToList();
            var bookingsModel = new List<BookingViewModel>();

            foreach(var booking in bookings)
            {
                var bookingModel = new BookingViewModel();
                _mapper.Map(booking, bookingModel);
                bookingsModel.Add(bookingModel);
            }

            return bookingsModel;
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
                booking.ApproveDisapproveBy = userId;

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
                    _emailService.SendEmail(userDetails.Email, subject, body);
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



    }
}
