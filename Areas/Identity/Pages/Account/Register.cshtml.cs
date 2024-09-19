// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OnAccount.Areas.Identity.Data;
using OnAccount.Models;
using OnAccount.Services;


namespace OnAccount.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
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
            //[Required]
            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 1)]
            [DisplayName("User Name:")]
            public string? ScreenName { get; set; } = "";

            [Required]
            [DataType(DataType.EmailAddress)]
            [StringLength(255, MinimumLength = 1)]
            [DisplayName("Email Address:")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 1)]
            [DisplayName("First Name:")]
            public string? FirstName { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 1)]
            [DisplayName("Last Name:")]
            public string? LastName { get; set; } = "";

            [Required]
            [DataType(DataType.PhoneNumber)]
            [StringLength(12, MinimumLength = 12)]
            [DisplayName("Phone Number:")]
            public string? PhoneNumber { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(150, MinimumLength = 1)]
            [DisplayName("Address:")]
            public string? Address { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, MinimumLength = 1)]
            [DisplayName("City:")]
            public string? City { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(2, MinimumLength = 2)]
            [DisplayName("State:")]
            public string? State { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            [StringLength(10, MinimumLength = 1)]
            [DisplayName("Zip:")]
            public string? Zip { get; set; } = "";

            [Required]
            [DataType(DataType.Date)]
            [DisplayName("Birthday:")]
            public string? DateofBirth { get; set; } = "";

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
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.ScreenName = Input.ScreenName;
                user.Email = Input.Email;
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;
                user.Address = Input.Address;
                user.City = Input.City; 
                user.State = Input.State;
                user.Zip = Input.Zip;
                user.DateofBirth = Input.DateofBirth;
                user.LastPasswordChangedDate = System.DateTime.Now;
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

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
                        DbConnectorService dbConnectorService = new DbConnectorService();
                        List<RoleModel> adminRolesList = dbConnectorService.GetUserRole("Administrator");
                    string adminEmailSubject = $"Account approval needed - OnAccount.net (Sponsored by MagnaDigi.com) {user.FirstName} {user.LastName}";
                    string adminEmailBody = ""; 
                    for (var i=0;i<adminRolesList.Count;i++)
                    {
                        adminEmailBody = $"Dear {adminRolesList[i].firstName} {adminRolesList[i].lastName} please unlock and assign a role to the user: {user.FirstName} {user.LastName} {user.ScreenName}";
                        await _emailSender.SendEmailAsync(adminRolesList[i].email,adminEmailSubject,adminEmailBody);
                    }
                    
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
