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

namespace ASI.Basecode.WebApp.Controllers
{
    public class RoomController : ControllerBase<RoomController>
    {
        private readonly IRoomService _roomservice;
        private readonly IAmenityService _amenityservice; //uncomment If need external amenityservice
        private readonly IRoomAmenityService _roomamenityservice;// uncomment If need external roomamenityservice

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public RoomController(IAmenityService amenityService,
                              IRoomAmenityService roomAmenityService,
                              IRoomService roomService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._roomservice = roomService;
            this._roomamenityservice = roomAmenityService;
            this._amenityservice = amenityService;
        }

        // GET: RoomController
        [HttpGet]
        public IActionResult Index()
        {
            var rooms = _roomservice.GetRooms();
            return View(rooms);
        }

        #region Get Methods
        [HttpGet]
        // GET: RoomController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: RoomController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: RoomController/Edit/5
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

            return View(room);
        }

        // GET: RoomController/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View();
        }
        #endregion


        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RoomViewModel model)
        {
            try
            {
                var roomAmenities = _roomamenityservice.GetRoomAmenities(model.RoomId);
                model.RoomAmenities = roomAmenities;
                // Get the list of selected amenities from the view (ViewData["RoomAmenitiesId"])
                var userSelectedAmenities = ViewData["RoomAmenitiesId"] as List<int> ?? new List<int>();

                // Get the list of amenity IDs currently associated with the room
                var currentAmenitiesIds = roomAmenities.Select(x => x.AmenityId).ToList();

                // 1. Identify amenities to remove (those in current list but not in the selected list)
                var amenitiesToRemove = currentAmenitiesIds.Except(userSelectedAmenities).ToList();

                // 2. Identify amenities to add (those in the selected list but not in the current list)
                var amenitiesToAdd = userSelectedAmenities.Except(currentAmenitiesIds).ToList();

                // Step 1: Remove old amenities
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
                TempData["ErrorMessage"] = ex.Message; ;
            }
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: RoomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
