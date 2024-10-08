// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using oa.Areas.Identity.Data;
using oa.Models;
using oa.Services;


namespace oa.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<AppUser> _userManager;
        public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            
            DbConnectorService dbConnectorService = new DbConnectorService();
            AppUserModel userModel = new AppUserModel();
            string? userScreeName = "";
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                userModel = dbConnectorService.GetUserDetailsById(userId);
                userScreeName = userModel.ScreenName;  //This is the login name for the user log.
                // all other properties are now available is the userModel object two lines above.
            }
            dbConnectorService.logModelCreator(userScreeName, " Logged out", "");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            
            if (returnUrl != null)
            {
                
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
