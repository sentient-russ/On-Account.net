// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using oa.Areas.Identity.Data;
using oa.Services;


namespace oa.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly DbConnectorService _dbConnectorService;
        public ForgotPasswordModel(UserManager<AppUser> userManager, IEmailSender emailSender, DbConnectorService dbConnectorService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _dbConnectorService = dbConnectorService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            [StringLength(100, ErrorMessage = "Please enter a valid date.", MinimumLength = 6)]
            public string SecurityQuestion { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Task<string> task = _dbConnectorService.GetUserDateOfBirthByEmailAsync(Input.Email);// returns MM-dd-yyyy
                string dob = await task;
                dob=dob.Replace("/","-");
                string yearOfInput = Input.SecurityQuestion.Substring(0,4);
                string inputDOB=Input.SecurityQuestion;
                int inputDOBLength = Input.SecurityQuestion.Length;
                string inputDob =(inputDOB.Substring(5)+"-"+yearOfInput);
                if (dob.Length == 9 && !inputDob[0].Equals('1')){
                    inputDob = inputDob.Substring(1);
                }
                bool dobBool= inputDob==dob;
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || (!(await _userManager.IsEmailConfirmedAsync(user) && inputDob == dob)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
