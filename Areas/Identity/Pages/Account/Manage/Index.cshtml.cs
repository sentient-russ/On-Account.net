// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using oa.Areas.Identity.Data;
using oa.Models;
using oa.Services;

namespace oa.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DbConnectorService _dbConnectorService;
        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            DbConnectorService dbConnectorService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbConnectorService = dbConnectorService;
        }


        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [NotMapped]
        public string File { get; set; } // Add this property

        public class InputModel
        {

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [NotMapped]
            [Display(Name = "UserName")]
            public string Username { get; set; }

            [BindProperty]
            public IFormFile FormFile { get; set; }

            [NotMapped]
            public string File { get; set; } = "";

        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Username = user.ScreenName

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            AppUserModel appUser = new AppUserModel();
            appUser = _dbConnectorService.GetUserDetailsById(user.Id);
            File = appUser.File;
            await LoadAsync(user);

            return Page(); //<-- error here
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (Input.FormFile != null && Input.FormFile.Length > 0)
            {
                // Check file length
                const long max_size = 3 * 1024 * 1024;
                if (Input.FormFile.Length > max_size)
                {
                    StatusMessage = "The file size must be less than 3MB.";
                    return RedirectToPage();
                }
                // Check file type
                var fileExtension = Path.GetExtension(Input.FormFile.FileName).ToLowerInvariant();
                if (fileExtension != ".jpg")
                {
                    StatusMessage = "The file type must be a .jpg type.";
                    return RedirectToPage();
                }
                // create a new file name for security purposes
                DateTimeOffset current_time = DateTimeOffset.UtcNow;
                long ms_time = current_time.ToUnixTimeMilliseconds();
                string new_file_name = ms_time.ToString() + ".jpg";

                // Save the data stream to a file
                var filePath = Path.Combine("wwwroot", "uploads", new_file_name);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.FormFile.CopyToAsync(stream);
                }
                // Save the new file name to the users DB record.
                user.ProfileImage = new_file_name;
                var setUserResult = await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
