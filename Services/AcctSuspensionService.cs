using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OnAccount.Areas.Identity.Data;
using OnAccount.Controllers;

/*
 This service runs once per hour to update scheduled account lockout events.
 */
namespace OnAccount.Services
{
    public class AcctSuspensionService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly ILogger<AdminController> logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private DbConnectorService _dbConnectorService;
        public AcctSuspensionService(ILogger<AdminController> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager) 
        {
            this.logger = logger;
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._userStore = userStore;
            this._signInManager = signInManager;
            this._dbConnectorService = new DbConnectorService();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(3600)); //runs on the hour
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
        private void DoWork(object? state)
        {
            UpdateLockouts();
        }
        private void UpdateLockouts()
        {
            var users = this._userManager.Users.ToList();
            for (var i =0; i < users.Count; i++)
            {
                if (users[i].AcctSuspensionDate >= System.DateTime.Now && users[i].AcctReinstatementDate >= System.DateTime.Now)
                {
                    _dbConnectorService.UpdateLockout(users[i]);
                }
            }
        }
    }
}

