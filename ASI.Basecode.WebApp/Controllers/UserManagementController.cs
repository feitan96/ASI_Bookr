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
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Superadmin")]

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
        private List<SelectListItem> GetRoles()
        {
            return Enum.GetValues(typeof(ASI.Basecode.Resources.Constants.Enums.Roles))
                       .Cast<ASI.Basecode.Resources.Constants.Enums.Roles>()
                       .Select(role => new SelectListItem
                       {
                           Value = role.ToString(),
                           Text = role.ToString()
                       }).ToList();
        }

        public IActionResult Index()
        {
            var users = _userService.GetAllUser();
            return View(users);
        }


        #region Get Methods
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = GetRoles();
            return PartialView("_Create");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.Roles = GetRoles();
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
            ViewBag.Roles = GetRoles();
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
            ViewBag.Roles = GetRoles();
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

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public IActionResult Promote(int id)
        {
            try
            {
                _userService.UpdateUserRole(id, "Admin");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public IActionResult Demote(int id)
        {
            try
            {
                _userService.UpdateUserRole(id, "User");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        #endregion
    }
}
