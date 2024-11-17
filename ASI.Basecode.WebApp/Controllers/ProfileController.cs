using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ASI.Basecode.Services.Services;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ProfileController : ControllerBase<ProfileController>
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;


        public ProfileController(
                            IProfileService profileService,
                            IUserService userService,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper, userService)
        {
            this._profileService = profileService;
            this._userService = userService;
        }
        public IActionResult Index()
        {
            if (Id == null) throw new ArgumentNullException("Logged in user ID not found.");
            var userIdClaim = int.Parse(Id);

            var user = _profileService.GetUser(userIdClaim);
            return View(user);
        }

        #region Get Methods
        [HttpGet]
        public IActionResult Edit()
        {
            if (Id == null) throw new ArgumentNullException("Logged in user ID not found.");
            var userIdClaim = int.Parse(Id);

            var data = _profileService.GetUser(userIdClaim);
            return PartialView("_Edit", data);
        }
        #endregion

        #region Posts Methods
        [HttpPost]
        public IActionResult Edit(ProfileViewModel model)
        {
            try
            {
                if (Id == null) throw new ArgumentNullException("Logged in user ID not found.");
                var userIdClaim = int.Parse(Id);

                _profileService.UpdateUser(model, userIdClaim);
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
        #endregion
    }
}
