using ASI.Basecode.WebApp.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Services;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Noding;
using ASI.Basecode.Data.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Core.Types;
using ASI.Basecode.Data;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "User")]

    public class UserRoomController : ControllerBase<UserRoomController>
    {
        private readonly IRoomService _roomservice;
        private readonly IAmenityService _amenityservice; //uncomment If need external amenityservice
        private readonly IRoomAmenityService _roomamenityservice;// uncomment If need external roomamenityservice
        private readonly IImageService _imageservice;
        private readonly IBookingService _bookingservice;
        private readonly IWebHostEnvironment _environment;
        private readonly BookrDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public UserRoomController(BookrDbContext context,
                              IAmenityService amenityService,
                              IRoomAmenityService roomAmenityService,
                              IRoomService roomService,
                              IImageService imageService,
                              IBookingService bookingService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWebHostEnvironment environment,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._roomservice = roomService;
            this._roomamenityservice = roomAmenityService;
            this._amenityservice = amenityService;
            this._environment = environment;  // Set the environment field
            this._imageservice = imageService;
            this._bookingservice = bookingService;
            this._context = context;
        }

        // GET: UserRoomController
        [HttpGet]
        public IActionResult Index()
        {
            var amenities = _amenityservice.GetAmenities();
            ViewData["AmenitiesList"] = amenities;

            var rooms = _roomservice.GetRooms();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult MyBookings()
        {
            var userId = int.Parse(Id);
            var bookings = _bookingservice.GetBookings().Where(booking => booking.UserId == userId).ToList();

            return View(bookings);
        }


        #region Get Methods
        //Create, Edit, View, Delete
        // GET: UserRoomController/Create
        [HttpGet]
        public IActionResult Create()
        {

            var amenities = _amenityservice.GetAmenities();

            ViewData["AmenitiesList"] = amenities;

            if (amenities == null)
            {
                throw new ArgumentNullException(nameof(amenities), "Amenities list is null");
            }
            return PartialView("_Create");
        }

        // GET: UserRoomController/Edit/5
        [HttpGet]
        public IActionResult Edit(int roomId)
        {
            var room = _roomservice.GetRoomById(roomId);

            if (room == null)
            {
                return NotFound();
            }

            var amenities = _amenityservice.GetAmenities();

            ViewData["AmenitiesList"] = amenities;

            if (amenities == null)
            {
                throw new ArgumentNullException(nameof(amenities), "Amenities list is null");
            }

            return PartialView("_Edit", room);
        }

        [HttpGet]
        // GET: UserRoomController/View/5
        public IActionResult View(int roomId) //For Viewing Rooms
        {
            var room = _roomservice.GetRoomById(roomId);

            if (room == null)
            {
                return NotFound();
            }
            return PartialView("_View", room);
        }

        [HttpGet]
        public IActionResult ViewBooking(int bookingId) //For Viewing Rooms
        {
            var booking = _bookingservice.GetBookings().Where(booking => booking.BookingId == bookingId).FirstOrDefault();

            if (booking == null)
            {
                return NotFound();
            }
            return PartialView("_ViewBooking", booking);
        }

        [HttpGet]
        public IActionResult CancelBooking(int bookingId) //For Viewing Rooms
        {
            var booking = _bookingservice.GetBookings().Where(booking => booking.BookingId == bookingId).FirstOrDefault();   
            if(booking == null)
            {
                NotFound();
            }
            return PartialView("_CancelBooking", booking);
        }

        [HttpGet]
        public IActionResult Book(int roomId)
        {
            ViewData["RoomId"] = roomId;
            
            return PartialView("_Book");
        }



        // GET: UserRoomController/Delete/5
        [HttpGet]
        public IActionResult Delete(int roomId)
        {
            var room = _roomservice.GetRoomById(roomId);

            if (room == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", room);
        }

        [HttpGet]
        public IActionResult Search(string roomName, string type, string location, int? capacity, [FromQuery(Name = "amenities[]")] List<int> amenities)
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

            return PartialView("_RoomList", rooms);
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
        //Create, Edit, SoftDelete, HardDelete
        // POST: UserRoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            try
            {
                //Create or Add new Room
                int roomId = _roomservice.AddRoom(model, int.Parse(Id));

                //list out the room amenities selected
                var userSelectedAmenities = Request.Form["RoomAmenitiesId"].Select(x => Convert.ToInt32(x)).ToList();  // Convert values to integers

                foreach (var amenityId in userSelectedAmenities)
                {
                    // Call the service to add the new mapping to RoomAmenities table for the newly created room
                    _roomamenityservice.AddRoomAmenity(roomId, amenityId);
                }

                if (ModelState.IsValid)
                {
                    var uploadedFiles = model.ImageFiles;

                    // Check if an image file was uploaded
                    if (uploadedFiles != null && uploadedFiles.Count > 0)
                    {
                        // Ensure the uploads directory exists
                        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads/rooms");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }

                        foreach (var uploadedFile in uploadedFiles)
                        {
                            // Generate a unique filename with GUID and get the extension
                            var fileExtension = Path.GetExtension(uploadedFile.FileName);
                            var fileName = $"{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadsDir, fileName);

                            // Save the file to the specified path
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(stream);
                            }


                            //Add image to database
                            ImageViewModel imageModel = new ImageViewModel(model.RoomId, fileName);
                            _imageservice.AddImage(imageModel);
                        }
                    }

                }
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

        // POST: UserRoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomViewModel model)
        {
            try
            {
                var roomAmenities = _roomamenityservice.GetRoomAmenities(model.RoomId);
                model.RoomAmenities = roomAmenities;
                // Get the list of selected amenities from the view (ViewData["RoomAmenitiesId"])
                var userSelectedAmenities = Request.Form["RoomAmenitiesId"].Select(x => Convert.ToInt32(x))  // Convert values to integers
            .ToList();
                // Get the list of amenity IDs currently associated with the room
                var currentAmenitiesIds = roomAmenities.Select(x => x.AmenityId).ToList();

                // Identify amenities to remove (those in current list but not in the selected list)
                var amenitiesToRemove = currentAmenitiesIds.Except(userSelectedAmenities).ToList();

                //Identify amenities to add (those in the selected list but not in the current list)
                var amenitiesToAdd = userSelectedAmenities.Except(currentAmenitiesIds).ToList();

                //Step 1: Remove old amenities
                foreach (var amenityId in amenitiesToRemove)
                {
                    // Call the service to remove the mapping from RoomAmenities table
                    _roomamenityservice.DeleteRoomAmenity(model.RoomId, amenityId);
                }

                // Step 2: Add new amenities
                foreach (var amenityId in amenitiesToAdd)
                {
                    // Call the service to add the new mapping to RoomAmenities table
                    _roomamenityservice.AddRoomAmenity(model.RoomId, amenityId);
                }

                if (ModelState.IsValid)
                {
                    var uploadedFiles = model.ImageFiles;

                    // Check if an image file was uploaded
                    if (uploadedFiles != null && uploadedFiles.Count > 0)
                    {
                        // Ensure the uploads directory exists
                        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads/rooms");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }

                        foreach (var uploadedFile in uploadedFiles)
                        {
                            // Generate a unique filename with GUID and get the extension
                            var fileExtension = Path.GetExtension(uploadedFile.FileName);
                            var fileName = $"{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadsDir, fileName);

                            // Save the file to the specified path
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(stream);
                            }

                            //Add image to database
                            ImageViewModel imageModel = new ImageViewModel(model.RoomId, fileName);
                            _imageservice.AddImage(imageModel);
                        }
                    }

                }

                // Step 3: Update the Room table if necessary (if any room-related changes were made)
                _roomservice.UpdateRoomInfo(model, int.Parse(Id));

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
            return View();
        }

        [HttpPost]
        public IActionResult Book(BookingViewModel model)
        {
            //bool successBooking = false;
            //Try not to use Model.IsValid as not all fields will be populated depending on whether booking is recurring or not.
            try
            {

                model.UserId = int.Parse(Id); //Get the current log-in user
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

        [HttpPost]
        public IActionResult CancelBooking(BookingViewModel model) //For Viewing Rooms
        {
            try
            {
                _bookingservice.UpdateBookingStatus(model, int.Parse(Id), "Cancelled"); //Cancel booking
                return Json(new
                {
                    success = true,
                    message = "Booking was cancelled successfully!",
                });
            }
            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return Json(new
            {
                success = false,
                message = "Failed to cancel your bookings. Please call the administrator."
            });
        }


        [HttpPost]
        public async Task<JsonResult> DeleteImage()
        {

            var jsonRequest = await JsonSerializer.DeserializeAsync<JsonElement>(Request.Body);

            // Log the raw JSON (optional)
            Console.WriteLine("Raw JSON Request: " + jsonRequest.ToString());


            // Manually extract the ImageId
            int? imageId = jsonRequest.GetProperty("imageId").GetInt32();

            if (imageId == 0 || imageId == null)
            {
                return Json(new { success = false, message = "Invalid ImageId." });
            }

            try
            {
                // Fetch the room image by roomId and imageId
                var image = _imageservice.GetImageById(imageId.Value);
                if (image == null)
                {
                    return Json(new { success = false, message = "Image not found." });
                }

                // Delete the image file from the server
                var imagePath = Path.Combine(_environment.WebRootPath, "uploads", "rooms", image.Guid);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Remove the image record from the database
                _imageservice.DeleteImageById(imageId.Value);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: UserRoomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SoftDelete(int roomId)
        {
            _roomservice.SoftDeleteRoom(roomId);
            return RedirectToAction("Index");
        }
        //unused
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HardDelete(int roomId)
        {
            _roomservice.HardDeleteRoom(roomId);
            return RedirectToAction("Index");
        }
        #endregion


    }
}
