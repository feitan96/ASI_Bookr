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
        private readonly IEmailService _emailService;

        public UserManagementController(
                            IUserService userService,
                            IEmailService emailService,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper) : base(httpContextAccessor, loggerFactory, configuration, mapper, userService)
        {
            this._userService = userService;
            this._emailService = emailService;
        }
        private List<SelectListItem> GetRoles(string userRole)
        {
            var allRoles = Enum.GetValues(typeof(ASI.Basecode.Resources.Constants.Enums.Roles))
                           .Cast<ASI.Basecode.Resources.Constants.Enums.Roles>()
                           .Select(role => new SelectListItem
                           {
                               Value = role.ToString(),
                               Text = role.ToString()
                           }).ToList();
            if (userRole == "Superadmin") return allRoles.Where(role => role.Value != "Superadmin").ToList();
            if (userRole == "Admin") return allRoles.Where(role => role.Value != "Superadmin" && role.Value != "Admin").ToList();

            return null;
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
            ViewBag.Roles = GetRoles(UserRole);
            return PartialView("_Create");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
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

        [HttpGet]
        public IActionResult Search(string name, string role)
        {
            var users = _userService.GetAllUser();

            if (!string.IsNullOrWhiteSpace(name))
            {
                users = users.Where(u => $"{u.FirstName} {u.LastName}".Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                users = users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return PartialView("_UserTable", users);
        }

        #endregion

        #region Posts Methods
        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            try
            {
                _userService.AddUser(model, int.Parse(Id));
                //return RedirectToAction("Index");
                return Json(new { success = true, successMessage = "User created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            try
            {
                _userService.UpdateUser(model, int.Parse(Id));
                //return RedirectToAction("Index");
                return Json(new { success = true, successMessage = "User updated successfully"});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SoftDelete(int Id)
        {
            _userService.SoftDelete(Id);
            //return RedirectToAction("Index");
            return Json(new { success = true, successMessage = "User removed successfully" });
        }
        //unused
        [HttpPost]
        public IActionResult HardDelete(int Id)
        {
            _userService.HardDelete(Id);
            //return RedirectToAction("Index");
            return Json(new { success = true, successMessage = "User deleted successfully" });
        }

        [HttpGet]
        [Authorize(Roles = "Superadmin")]
        public IActionResult ChangeRoleModal(int id)
        {
            var user = _userService.GetUser(id);
            if (user == null)
                return NotFound("User not found.");

            return PartialView("_ChangeRoleModal", user);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public IActionResult Promote(int id)
        {
            try
            {
                // Get the user details before updating
                var user = _userService.GetUser(id);

                // Update user role
                _userService.UpdateUserRole(id, "Admin");

                // Send email notification
                string subject = "Role Change Notification";
                string body = $"Good day, {user.FirstName} {user.LastName}. You have been Promoted to Admin. Thank you.";

                _emailService.SendEmail(user.Email, subject, body);

                return Json(new { success = true, successMessage = "User promoted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public IActionResult Demote(int id)
        {
            try
            {
                // Get the user details before updating
                var user = _userService.GetUser(id);

                // Update user role
                _userService.UpdateUserRole(id, "User");

                // Send email notification
                string subject = "Role Change Notification";
                string body = $"Good day, {user.FirstName} {user.LastName}. You have been Demoted to User. Thank you.";

                _emailService.SendEmail(user.Email, subject, body);

                return Json(new { success = true, successMessage = "User demoted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        #endregion
    }
}
