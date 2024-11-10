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
    public class UserManagementController : ControllerBase<UserManagementController>
    {
        private readonly IUserService _userService;

        public UserManagementController(
                            IUserService userService,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper) : base(httpContextAccessor, loggerFactory, configuration, mapper, userService)
        {
            this._userService = userService;
        }
        private List<SelectListItem> Roles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "User", Text = "User" },
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "SuperAdmin", Text = "SuperAdmin" }
            };
        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 1)
        {
            var pagedUsers = _userService.GetAllUsers(pageNumber, pageSize);
            return View(pagedUsers);
        }


        #region Get Methods
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = Roles();
            return PartialView("_Create");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.Roles = Roles();
            var data = _userService.GetUser(Id);
            return PartialView("_Edit", data);
        }

        [HttpGet]
        public IActionResult View(int Id)
        {
            var data = _userService.GetUser(Id);
            return PartialView("_View", data);
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var data = _userService.GetUser(Id);
            return PartialView("_Delete", data);
        }
        #endregion

        #region Posts Methods
        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            try
            {
                _userService.AddUser(model, int.Parse(Id));
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
            ViewBag.Roles = Roles();
            return View();
        }

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
            ViewBag.Roles = Roles();
            return View();
        }

        [HttpPost]
        public IActionResult SoftDelete(int Id)
        {
            _userService.SoftDelete(Id);
            return RedirectToAction("Index");
        }
        //unused
        [HttpPost]
        public IActionResult HardDelete(int Id)
        {
            _userService.HardDelete(Id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
