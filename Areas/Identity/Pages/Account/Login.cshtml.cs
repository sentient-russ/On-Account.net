// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using oa.Areas.Identity.Data;
using oa.Services;


namespace oa.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DbConnectorService _dbConnectorService;

        public LoginModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<LoginModel> logger, RoleManager<IdentityRole> roleManager, DbConnectorService connectorService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _dbConnectorService = connectorService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "Please enter a valid email address or username.", MinimumLength = 1)]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // apply lockout status

            //If a string other than an email is entered check to see if it is a valid username.
            //If it is a username
            //look up the email
            //use the email to validate the account.
            var email = Input.Email;
            if (!email.Contains("@"))
            {
                // Input.Email variable is a string that is not a valid email address at this point so check to see if it is a valid username.

                //if the username is not found the email a "" or null will be returned.
                //otherwise a valid elmail will be return which can be replace the username in the input email field.
                if (email == "" || email == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid user name or email!");
                    return Page();
                }
                else
                {
                    //replace the screenname with the actual email address and continue to validate the account based on the email.
                    email = _dbConnectorService.GetUserEmail(email);
                    Input.Email = email;
                }
            }
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user != null)
            {
                // directs the user to the lockout page if the account is locked.
                if (user.LockoutEnabled)
                {
                    string lockoutMessage = "Locked out user attempted login: " + user.Id.ToString();
                    _logger.LogWarning(lockoutMessage);
                    ModelState.AddModelError(string.Empty, "Account locked out.");
                    _dbConnectorService.logModelCreator(user.ScreenName,"Locked user logged in","");
                    return RedirectToPage("./Lockout");
                }
                // locks the user out if the password has expired.
                DateTime lastChangedDate = (DateTime)user.LastPasswordChangedDate;
                var notificationDate = lastChangedDate.AddDays(90);
                if (notificationDate <= DateTime.Now)
                {
                    string expiredPasswordLockoutMessage = "User account locked out due to expired password: " + user.Id.ToString();
                    ModelState.AddModelError(string.Empty, "Account disabled.");
                    _logger.LogWarning(expiredPasswordLockoutMessage);
                    _dbConnectorService.logModelCreator(user.ScreenName, "user with expired password logged", "");
                    return RedirectToPage("./Lockout");
                }
                // directs new unaproved accounts to a "Awaiting confirmation" message if their acount has not been approved.
                if (user.UserRole == null || user.UserRole == "")
                {
                    returnUrl += "Home/FirstLogin";
                    _dbConnectorService.logModelCreator(user.ScreenName, "unapproved account logged in", "");
                    return LocalRedirect(returnUrl);
                }
                // checks to make sure the account is not suspended by date range.
                if (user.AcctSuspensionDate <= System.DateTime.Now)
                {
                    if (user.AcctReinstatementDate >= System.DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, "Account under temporary suspension.");
                        _dbConnectorService.logModelCreator(user.ScreenName, "temporary suspension logged in", "");
                        return RedirectToPage("./Lockout");
                    }
                }

            }
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User authenticated. Checking authorization...");
                    DateTime lastChangedDate = (DateTime)user.LastPasswordChangedDate;
                    var notificationDate = lastChangedDate.AddDays(90);
                    // redirects the user to a password reset if it is set to expire in the next three days.                    
                    notificationDate = lastChangedDate.AddDays(87);
                    if (notificationDate <= DateTime.Now)
                    {
                        return RedirectToPage("./Manage/PasswordExpired");
                    }
                    else
                    {
                        // redirects users that have the Manager or Accountant roles to the accounting application
                        if (user.UserRole != null && user.UserRole != "")
                        {
                            if (user.UserRole == "Manager" || user.UserRole == "Accountant")
                            {
                                returnUrl += "Accounting/";
                                _dbConnectorService.logModelCreator(user.ScreenName, user.UserRole+" logged in", "");
                                return LocalRedirect(returnUrl); //working
                            }
                            else if (user.UserRole == "Administrator")
                            {
                                returnUrl += "Admin/Index";
                                _dbConnectorService.logModelCreator(user.ScreenName, user.UserRole + " logged in", "");
                                return LocalRedirect(returnUrl); //working
                            }
                        }
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();

        }
    }
}
