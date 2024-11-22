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
using Serilog.Core;
using System.Text.Json;

namespace ASI.Basecode.WebApp.Controllers
{
    public class BookingController : ControllerBase<BookingController>
    {
        private readonly IBookingService _bookingservice;
        private readonly IRoomService _roomservice;
        private readonly IAmenityService _amenityservice;
        private readonly IUserService _userservice;
        private readonly IEmailService _emailService;
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
                              IEmailService emailService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWebHostEnvironment environment,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._roomservice = roomService;
            this._userservice = userService;
            this._emailService = emailService;
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
            //var booking = _bookingservice.GetBooking(bookingId);
            //return PartialView("_ApproveBooking", bookingId);
            ViewData["bookingId"] = bookingId;
            return PartialView("_ApproveBooking");
        }

        [HttpGet]
        public IActionResult Disapprove(int bookingId)
        {
            //var booking = _bookingservice.GetBooking(bookingId);
            //return PartialView("_DisapproveBooking", bookingId);
            ViewData["bookingId"] = bookingId;
            return PartialView("_DisapproveBooking");
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

        [HttpGet]
        public IActionResult ShowConflictsAndSuggestions(string conflictBookingIds, string roomSuggestionIds, string bookingData)
        {
            if (string.IsNullOrEmpty(conflictBookingIds))
            {
                return BadRequest("No conflict booking IDs provided.");
            }

            // Deserialize the JSON array of conflict booking IDs
            var bookingIds = JsonSerializer.Deserialize<List<int>>(conflictBookingIds);
            var roomIds = JsonSerializer.Deserialize<List<int>>(roomSuggestionIds);

            // Deserialize the bookingData JSON into the BookingViewModel
            BookingViewModel bookingViewModel = null;
            if (!string.IsNullOrEmpty(bookingData))
            {
                try
                {
                    // Manually mapping the values from the JSON string to the BookingViewModel properties
                    var bookingDataJson = JsonSerializer.Deserialize<JsonElement>(bookingData);

                    bookingViewModel = new BookingViewModel
                    {
                        // Convert RoomId from string to integer using int.Parse (or int.TryParse for safety)
                        RoomId = int.TryParse(bookingDataJson.GetProperty("RoomId").GetString(), out int roomId) ? roomId : 0,
                        UserId = int.TryParse(bookingDataJson.GetProperty("UserId").GetString(), out int userId) ? userId : 0,
                        Status = bookingDataJson.GetProperty("Status").GetString(),
                        Title = bookingDataJson.GetProperty("Title").GetString(),
                        BookingStartDate = DateTime.Parse(bookingDataJson.GetProperty("BookingStartDate").GetString()),
                        CheckInTimeString = bookingDataJson.GetProperty("CheckInTimeString").GetString(),
                        CheckOutTimeString = bookingDataJson.GetProperty("CheckOutTimeString").GetString(),
                        // Convert IsRecurring from string to boolean using bool.TryParse
                        IsRecurring = bool.TryParse(bookingDataJson.GetProperty("IsRecurring").GetString(), out bool isRecurring) ? isRecurring : false,
                        BookingEndDate = DateTime.TryParse(bookingDataJson.GetProperty("BookingEndDate").GetString(), out DateTime endDate) ? endDate : (DateTime?)null,
                        SelectedDays = bookingDataJson.GetProperty("SelectedDays").GetString()
                    };
                }
                catch (JsonException ex)
                {
                    return BadRequest($"Error deserializing booking data: {ex.Message}");
                }

            }


            // Fetch conflict bookings using the provided IDs
            var conflictBookings = _bookingservice.GetBookingsByIds(bookingIds);
            var suggestedRooms = _roomservice.GetRoomsByIds(roomIds);

            ViewData["SuggestedRooms"] = suggestedRooms;
            ViewData["BookingModel"] = bookingViewModel;

            return PartialView("_ConflictAndSuggestions", conflictBookings);
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
                _bookingservice.UpdateBookingStatus(model, int.Parse(Id), "Approved");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to disapprove booking. Please try again.";
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult ApproveBookingById(int bookingId)
        {
            //Uncomment the following code if NoTracking option is preferred
            //var bookingid = model.bookingid;
            var bookingModel = _bookingservice.GetBookingByIdNoTracking(bookingId);
            try
            {
                _bookingservice.UpdateBookingStatus(bookingModel, int.Parse(Id), "Approved");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to disapprove booking. Please try again.";
                return View(bookingModel);
            }
        }

        [HttpPost]
        public IActionResult DisapproveBookingById(int bookingId)
        {
            //Uncomment the following code if NoTracking option is preferred
            //var bookingid = model.bookingid;
            var bookingModel = _bookingservice.GetBookingByIdNoTracking(bookingId);
            try
            {
                _bookingservice.UpdateBookingStatus(bookingModel, int.Parse(Id), "Disapproved");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to disapprove booking. Please try again.";
                return View(bookingModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DisapproveBooking(BookingViewModel model)
        {
            try
            {
                _bookingservice.UpdateBookingStatus(model, int.Parse(Id), "Disapproved");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to approve booking. Please try again.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(BookingViewModel model)
        {
            //bool successBooking = false;
            //Try not to use Model.IsValid as not all fields will be populated depending on whether booking is recurring or not.
            try
            {
                List<BookingViewModel> conflictBookings = _bookingservice.GetConflictBookings(model, null);

                if (conflictBookings == null || !conflictBookings.Any())
                {
                    // No conflicts, proceed to book
                    _bookingservice.AddBooking(model);

                    // Return JSON success message
                    return Json(new
                    {
                        success = true,
                        message = "Booking was successful!",
                    });
                }
                else
                {
                    // There are conflicts, return conflict details
                    var conflictBookingIds = conflictBookings.Select(b => b.BookingId).ToList();
                    var roomSuggestions = _bookingservice.GetAvailableRoomsForBooking(model);
                    var roomSuggestionIds = roomSuggestions.Select(room => room.RoomId).ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Booking failed due to conflicts.",
                        conflictBookingIds = conflictBookingIds,
                        roomSuggestionIds = roomSuggestionIds
                    });
                }
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdatedBook(string bookingData, int updatedRoomId)
        {
            BookingViewModel model = null;
            try
            {
                var bookingDataJson = JsonSerializer.Deserialize<JsonElement>(bookingData);

                model = new BookingViewModel
                {
                    // Use the exact property names from the JSON
                    RoomId = bookingDataJson.GetProperty("roomId").GetInt32(),
                    UserId = bookingDataJson.GetProperty("userId").GetInt32(),
                    Status = bookingDataJson.GetProperty("status").GetString(),
                    Title = bookingDataJson.GetProperty("title").GetString(),
                    BookingStartDate = DateTime.Parse(bookingDataJson.GetProperty("bookingStartDate").GetString()),
                    CheckInTimeString = bookingDataJson.GetProperty("checkInTimeString").GetString(),
                    CheckOutTimeString = bookingDataJson.GetProperty("checkOutTimeString").GetString(),
                    IsRecurring = bookingDataJson.GetProperty("isRecurring").GetBoolean(),
                    BookingEndDate = DateTime.Parse(bookingDataJson.GetProperty("bookingEndDate").GetString()),
                    SelectedDays = bookingDataJson.GetProperty("selectedDays").GetString()
                };
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
            model.RoomId = updatedRoomId; //Change the RoomId
            return this.Book(model); //Reuse the Book method with the updated room Id
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
