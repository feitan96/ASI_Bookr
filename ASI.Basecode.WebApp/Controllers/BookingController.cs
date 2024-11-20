using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using ASI.Basecode.Services.ServiceModels;
using System.IO;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.WebApp.Controllers
{
    public class BookingController : ControllerBase<BookingController>
    {
        private readonly IBookingService _bookingservice;
        private readonly IRoomService _roomservice;
        private readonly IAmenityService _amenityservice;
        private readonly IUserService _userservice;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public BookingController(IBookingService bookingService,
                              IAmenityService amenityService,
                              IRoomService roomService,
                              IUserService userService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWebHostEnvironment environment,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._roomservice = roomService;
            this._userservice = userService;
            this._amenityservice = amenityService;
            this._bookingservice = bookingService;
            this._environment = environment;  // Set the environment field
        }


        // GET: BookingController
        [HttpGet]
        public IActionResult Index()
        {
            var rooms = _roomservice.GetRooms();
            ViewData["RoomsList"] = rooms;

            var users = _userservice.GetAllUser();
            ViewData["UsersList"] = users;

            var bookings = _bookingservice.GetBookings();
            return View(bookings);
        }

        #region Get Methods
        //Create, Edit, View, Delete
        // GET: BookingController/Create
        [HttpGet]
        public IActionResult Create()
        {
            //Uncomment if needed, index view already loads the following data
            //var rooms = _roomservice.GetRooms();
            //ViewData["RoomsList"] = rooms;

            ////Selection for users
            //var users = _userservice.GetAllUser();
            //ViewData["UsersList"] = users;

            return PartialView("_Create");
        }

        // GET: BookingController/Edit/5
        [HttpGet]
        public IActionResult Edit(int bookingId)
        {
            //Uncomment if needed, index view already loads the following data
            //var users = _userservice.GetAllUser();
            //ViewData["UsersList"] = users;

            //var rooms = _roomservice.GetRooms();
            //ViewData["RoomsList"] = rooms;

            var booking = _bookingservice.GetBooking(bookingId);

            return PartialView("_Edit", booking);
        }

        // GET: BookingController/View/5
        [HttpGet]
        public IActionResult View(int bookingId)
        {
            var booking = _bookingservice.GetBooking(bookingId);

            return PartialView("_View", booking);
        }

        // GET: BookingController/Delete/5
        [HttpGet]
        public IActionResult Delete(int bookingId)
        {
            var booking = _bookingservice.GetBooking(bookingId);

            return PartialView("_Delete", booking);
        }

        [HttpGet]
        public IActionResult Approve(int bookingId)
        {
            var booking = _bookingservice.GetBooking(bookingId);

            return PartialView("_ApproveBooking", booking);
        }

        [HttpGet]
        public IActionResult Disapprove(int bookingId)
        {
            var booking = _bookingservice.GetBooking(bookingId);

            return PartialView("_DisapproveBooking", booking);
        }

        [HttpGet]
        public IActionResult RoomSelection()
        { 
            var rooms = _roomservice.GetRooms();

            //Get amenities for search filtering (if implemented)
            var amenities = _amenityservice.GetAmenities();
            ViewData["AmenitiesList"] = amenities;

            return PartialView("_RoomSelection", rooms);
        }

        [HttpGet]
        public IActionResult UserSelection()
        {
            var users = _userservice.GetAllUser();

            return PartialView("_UserSelection", users);
        }

        [HttpGet]
        public IActionResult SearchRooms(string roomName, string type, string location, int? capacity, [FromQuery(Name = "amenities[]")] List<int> amenities)
        {

            var rooms = _roomservice.GetRooms();

            // Step 1: Filter by Room Name
            if (!string.IsNullOrEmpty(roomName))
            {
                rooms = rooms.Where(r => r.Name.Contains(roomName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Step 2: Filter by Type
            if (!string.IsNullOrEmpty(type))
            {
                rooms = rooms.Where(r => r.Type == type).ToList();
            }

            // Step 3: Filter by Location
            if (!string.IsNullOrEmpty(location))
            {
                rooms = rooms.Where(r => r.Location == location).ToList();
            }

            // Step 4: Filter by Capacity
            if (capacity.HasValue)
            {
                rooms = rooms.Where(r => r.Capacity >= capacity.Value).ToList();
            }

            // Step 5: Filter by Amenities
            if (amenities != null && amenities.Any())
            {
                rooms = rooms.Where(r => amenities.All(a => r.RoomAmenitiesId.Contains(a))).ToList();
            }

            return PartialView("_RoomListSelection", rooms);
        }

        [HttpGet]
        public IActionResult SearchUsers(string searchBy, string searchTerm)
        {
            var users = _userservice.GetAllUser();

            // Filter users based on search criteria
            if (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchTerm))
            {
                switch (searchBy.ToLower())
                {
                    case "id":
                        if (int.TryParse(searchTerm, out int userId))
                        {
                            users = users.Where(user => user.Id == userId).ToList();
                        }
                        break;

                    case "email":
                        users = users.Where(user => user.Email != null && user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;

                    case "firstname":
                        users = users.Where(user => user.FirstName != null && user.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;

                    case "lastname":
                        users = users.Where(user => user.LastName != null && user.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;

                    default:
                        return BadRequest("Invalid search criteria.");
                }
            }

            return PartialView("_UserListSelection", users);
        }

        [HttpGet]
        public IActionResult SearchBookings(string searchText, string searchBy, string startDateTime, string endDateTime)
        {
            var bookings = _bookingservice.GetBookings();

            // Filter by searchText and searchBy if provided
            if (!string.IsNullOrWhiteSpace(searchText) && !string.IsNullOrWhiteSpace(searchBy))
            {
                if (searchBy == "user")
                {
                    // Try parsing searchText as UserId
                    if (int.TryParse(searchText, out int userId))
                    {
                        bookings = bookings.Where(b => b.UserId == userId).ToList();
                    }
                    else
                    {
                        // Search by FirstName or LastName
                        bookings = bookings.Where(b =>
                            b.User.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            b.User.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
                else if (searchBy == "room")
                {
                    // Try parsing searchText as RoomId
                    if (int.TryParse(searchText, out int roomId))
                    {
                        bookings = bookings.Where(b => b.RoomId == roomId).ToList();
                    }
                    else
                    {
                        // Search by RoomName
                        bookings = bookings.Where(b =>
                            b.Room.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
            }

            if (DateTime.TryParse(startDateTime, out var start) &&
                DateTime.TryParse(endDateTime, out var end))
            {
                // Filter by date range if both startDateTime and endDateTime are provided
                bookings = bookings.Where(b =>
                        b.BookingStartDate < end &&
                        b.BookingEndDate > start).ToList();
            }
               


            return PartialView("_BookingList", bookings);
        }


        #endregion


        #region Post Methods
        //Create, Edit, Delete
        // POST: BookingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookingViewModel model)
        {
            try
            {
                _bookingservice.AddBooking(model);


                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View();
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookingViewModel model)
        {
            try
            {
                // Print the entire form data
                //var formData = Request.Form;

                //foreach (var key in formData.Keys)
                //{
                //    Console.WriteLine($"{key}: {formData[key]}");
                //}
                _bookingservice.UpdateBookingInfo(model, int.Parse(Id));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveBooking(BookingViewModel model)
        {
            try
            {
                int adminId = int.Parse(Id);
                model.ApproveDisapproveBy = adminId;
                model.Status = "Approved";
                _bookingservice.UpdateBookingInfo(model, int.Parse(Id));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DisapproveBooking(BookingViewModel model)
        {
            try
            {
                model.Status = "Disapproved";
                _bookingservice.UpdateBookingInfo(model, int.Parse(Id));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // POST: BookingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HardDelete(int bookingId)
        {
            _bookingservice.HardDeleteBooking(bookingId);
            return RedirectToAction("Index");
        }

        #endregion
    }
}
