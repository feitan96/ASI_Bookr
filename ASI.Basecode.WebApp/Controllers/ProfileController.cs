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

namespace ASI.Basecode.WebApp.Controllers
{
    public class ProfileController : ControllerBase<ProfileController>
    {
        private readonly IUserService _userService;

        public ProfileController(
                            IUserService userService,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper) : base(httpContextAccessor, loggerFactory, configuration, mapper, userService)
        {
            this._userService = userService;
        }
        public IActionResult Index(int Id)
        {
            var user = _userService.GetUser(Id);
            return View(user);
        }


        #region Get Methods
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _userService.GetUser(Id);
            return PartialView("_Edit", data);
        }
        #endregion

        #region Posts Methods
        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            try
            {
                _userService.UpdateUser(model, int.Parse(Id));
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;;
            }
            return View();
        }
        #endregion
    }
}
