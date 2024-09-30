// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using oa.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;
using Microsoft.CodeAnalysis.Editing;
using oa.Services;


namespace oa.Areas.Identity.Pages.Account
{
    public class AdminRegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRegisterModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            static DateTime today = System.DateTime.Now;
            static DateTime birthdayPlaceholder = today.AddYears(-100);

            [Required]
            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 1)]
            [DisplayName("(automatic) User Name:")]
            public string? ScreenName { get; set; } = "";
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "First name must be under 100 characters long.", MinimumLength = 1)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "Last name must be under 100 characters long.", MinimumLength = 1)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = "";

            [Required]
            [DataType(DataType.PhoneNumber)]
            [StringLength(12, ErrorMessage = "Phone number must be 10 digits.", MinimumLength = 12)]
            [DisplayName("Phone Number:")]
            public string? PhoneNumber { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "Address must be under 100 characters long.", MinimumLength = 1)]
            [Display(Name = "Current Address")]
            public string Address { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "Address must be under 50 characters long.", MinimumLength = 1)]
            [DisplayName("City:")]
            public string? City { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(2, ErrorMessage = "Address must 2 characters long.", MinimumLength = 2)]
            [DisplayName("State:")]
            public string? State { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(5, ErrorMessage = "Please enter a 5 digit zip code.", MinimumLength = 1)]
            [DisplayName("Zip:")]

            public string? Zip { get; set; } = "";
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateofBirth { get; set; } = birthdayPlaceholder;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [FirstLetterCapitalValidator(ErrorMessage = "The first letter of the password must be capitalized.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            //[StringLength(100, MinimumLength = 4)]
            [DisplayName("Suspension Date:")]
            public DateTime? AcctSuspensionDate { get; set; }

            [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            //[StringLength(100, MinimumLength = 4)]
            [DisplayName("Reinstatement Date:")]
            public DateTime? AcctReinstatementDate { get; set; }

            [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            //[StringLength(100, MinimumLength = 4)]
            [DisplayName("Last Password Change:")]
            public string? LastPasswordChangedDate { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 1)]
            [DisplayName("Next Reset Days:")]
            public string PasswordResetDays { get; set; } = "90";

            [Required]
            [Display(Name = "User Role")]
            public string UserRole { get; set; } = "";

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })

            };
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.ScreenName = Input.ScreenName;
                user.Address = Input.Address;
                user.City = Input.City;
                user.State = Input.State;
                user.Zip = Input.Zip;
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.DateofBirth = Input.DateofBirth;
                user.Email = Input.Email;
                user.PhoneNumber = Input.PhoneNumber;
                user.AcctSuspensionDate = Input.AcctSuspensionDate;
                user.AcctReinstatementDate = Input.AcctReinstatementDate;
                user.UserRole = Input.UserRole;
                user.LastPasswordChangedDate = System.DateTime.Now;
                user.PasswordResetDays = Input.PasswordResetDays;
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, Input.UserRole);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Email confirmation from OnAccount.net (Sponsored by MagnaDigi.com)",
                        $"<center><img src='https://on-account.net/img/onaccount_logo.jpg'></center><p>Welcome to the crew!</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
