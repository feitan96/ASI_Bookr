using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class AmenityController : ControllerBase<AmenityController>
    {
        private readonly IAmenityService _amenityservice;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AmenityController(IAmenityService amenityService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._amenityservice = amenityService;
        }


        // GET: AmenityController
        [HttpGet]
        public IActionResult Index()
        {
            var amenities = _amenityservice.GetAmenities();
            return View(amenities);
        }

        // GET: AmenityController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        // GET: AmenityController/Edit/5
        [HttpGet]
        public IActionResult Edit(int amenityId)
        {
            var amenity = _amenityservice.GetAmenity(amenityId);
            return PartialView("_Edit", amenity);
        }

        // GET: AmenityController/View/5
        [HttpGet]
        public IActionResult View(int amenityId)
        {
            var amenity = _amenityservice.GetAmenity(amenityId);
            if (amenity == null)
            {
                return NotFound();
            }
            return PartialView("_View", amenity);
        }

        // GET: AmenityController/Delete/5
        [HttpGet]
        public IActionResult Delete(int amenityId)
        {
            var amenity = _amenityservice.GetAmenity(amenityId);
            if (amenity == null)
            {
                return NotFound();
            }
            return PartialView("_Delete", amenity);
        }

        // POST: AmenityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AmenityViewModel model)
        {
            try
            {
                _amenityservice.AddAmenity(model); //add new amenity to database
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

        #region Post Methods
        // POST: AmenityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AmenityViewModel model)
        {
            try
            {
                _amenityservice.UpdateAmenityInfo(model);
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

        // POST: AmenityController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HardDelete(int amenityId)
        {
            try
            {
                _amenityservice.DeleteAmenity(amenityId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion
    }
}
