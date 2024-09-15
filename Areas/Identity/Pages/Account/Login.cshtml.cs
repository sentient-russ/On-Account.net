// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnAccount.Areas.Identity.Data;
using OnAccount.Services;
using System.Security.Claims;
using System.Runtime.ConstrainedExecution;
using Microsoft.Identity.Client;

namespace OnAccount.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<LoginModel> logger, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
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
            [StringLength(50, ErrorMessage = "Please enter a valid User Name or registration email address.", MinimumLength = 1)]
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
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //check to see if user entered an email address.  If not check to see if it is a valide username.
                DbConnectorService db = new DbConnectorService();
                if (!Input.Email.Contains("@")){
                    var email = db.GetUserEmail(Input.Email);
                    if (email == "" || email == null)
                    {
                        // do nothing
                    } else {
                        Input.Email = email;
                    }
                }
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User authenticated. Checking authorization...");
                    // redirects the user to a password reset if it is set to expire in the next three days.
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    DateTime lastChangedDate = (DateTime)user.LastPasswordChangedDate;
                    var notificationDate = lastChangedDate.AddDays(90);
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
                                return LocalRedirect(returnUrl); //working
                            }
                            else if (user.UserRole == "Administrator")
                            {
                                returnUrl += "Admin/ManageAccounts";
                                return LocalRedirect(returnUrl); //working
                            }
                        }
                        else if (user.UserRole == null || user.UserRole == "")
                        {
                            returnUrl += "Home/FirstLogin";
                            return LocalRedirect(returnUrl);  //working
                        }
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }

        // If we got this far, something failed, redisplay form
        return Page();
            
        }
    }
}
