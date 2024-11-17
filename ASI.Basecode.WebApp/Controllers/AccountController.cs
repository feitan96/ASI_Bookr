using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            IEmailService emailService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
            this._emailService = emailService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            User user = new() { Id = 0, Email = "email@gmail.com", FirstName = "First Name", LastName = "Last Name", Password = "Password" };

            if (model == null) throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            var loginResult = _userService.AuthenticateUser(model.Email, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                // Sign in the user
                await this._signInManager.SignInAsync(user);
                this._session.SetString("Name", $"{user.FirstName} {user.LastName}");

                // Redirect based on role
                if (User.IsInRole("Admin") || User.IsInRole("Superadmin"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (User.IsInRole("User"))
                {
                    return RedirectToAction("Index", "Room");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect Email or Password";
            }

            return View();
        }

        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Redirect to Forgot Password Page.
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Redirect to Reset Password Page.
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            var tokenResult = _userService.IsTokenValid(token);
            if (tokenResult == ChangePassToken.Invalid)
            {
                TempData["ErrorMessage"] = "Token expired or invalid";
                return RedirectToAction("Login", "Account");
            }

            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        /// <summary>
        /// Send Request link for Changing Password.
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RequestPasswordReset(string email)
        {
            var token = _userService.GeneratePasswordResetToken(email);
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Email not found. Please check the email address.";
                return RedirectToAction("ForgotPassword", "Account");
            }
            _emailService.SendPasswordResetEmail(email, token);
            TempData["SuccessMessage"] = "Password reset link has been sent to your email address.";
            return RedirectToAction("ForgotPassword", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            var result = _userService.ResetPassword(model);
            if (result == Status.Error)
            {
                TempData["ErrorMessage"] = "Failed to reset password.";
                return View(model);
            }
            TempData["SuccessMessage"] = "Password reset successful.";
            return RedirectToAction("Login", "Account");
        }
    }
}
