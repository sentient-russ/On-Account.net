using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oa.Areas.Identity.Data;
using oa.Services;
using oa.Models;
using System.ComponentModel;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Rendering;
using oa.Areas.Identity.Services;
namespace OnAccount.Controllers
{
    [Authorize(Roles = "Administrator")]  //[Authorize(Roles = "Administrator,Accountant")] for multiple role assignment
    [BindProperties(SupportsGet = true)]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _DbContext;
        private readonly DbConnectorService _dbConnectorService;

        public AdminController(ILogger<AdminController> logger, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            DbConnectorService dbConnectorService,
            IEmailSender EmailSender)
        {
            this.logger = logger;
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._userStore = userStore;
            this._signInManager = signInManager;
            this._DbContext = context;
            this._dbConnectorService = dbConnectorService;
            this._emailSender = EmailSender;
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            
            return View();
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult ManageRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> ManageAccounts()
        {
            var appUsers = await _DbContext.Users.ToListAsync();
            return View(appUsers);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> EditAccountDetails(string? Id)
        {
            var appUsers = await _DbContext.Users.ToListAsync();
            AppUser appUser = new AppUser();
            for (var i = 0; i < appUsers.Count; i++)
            {
                if (appUsers[i].Id == Id)
                {
                    appUser = appUsers[i];
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var items = new List<SelectListItem>();
            foreach (var role in roles)
            {
                items.Add(new SelectListItem
                {
                    Text = role.Name,  
                });
            }
            appUser.RoleList = items;
            return View(appUser);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> UpdateAccountDetails([Bind("Id, ScreenName, FirstName, LastName, PhoneNumber, Address, City, State, Zip, DateofBirth, UserRole, ActiveStatus, UserName, Email, NormalizedUserName, AcctSuspensionDate, AcctReinstatementDate, LastPasswordChangedDate, PasswordResetDays")] AppUserModel detailsIn)
        {
            AppUserModel appUser = new AppUserModel();
            appUser = detailsIn;
            _dbConnectorService.UpdateUserDetails(appUser);
            var user = await _userManager.FindByIdAsync(appUser.Id);


            if (user != null)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, appUser.UserRole);
            }
            return RedirectToAction(nameof(ManageAccounts));
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Email(string? Id)
        {
            var appUsers = await _DbContext.Users.ToListAsync();
            AppUser appUser = new AppUser();
            for (var i = 0; i < appUsers.Count; i++)
            {
                if (appUsers[i].Id == Id)
                {
                    appUser = appUsers[i];
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var items = new List<SelectListItem>();
            foreach (var role in roles)
            {
                items.Add(new SelectListItem
                {
                    Text = role.Name,
                });
            }
            appUser.RoleList = items;
            return View(appUser);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> SendEmail([Bind("Id, Email, Subject, Message")] AppUserModel detailsIn)
        {
            AppUserModel appUserMessage = new AppUserModel();
            appUserMessage = detailsIn;
            _ = Task.Run(() => _emailSender.SendEmailAsync(appUserMessage.Email, appUserMessage.Subject, appUserMessage.Message));
            return RedirectToAction(nameof(ManageAccounts));
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Lock(string? Id)
        {
            AppUserModel userModel = new AppUserModel();
            AppUserModel lockedAccountModel = _dbConnectorService.GetUserDetailsById(Id);
            string? userScreenName = "";
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                userModel = _dbConnectorService.GetUserDetailsById(userId);
                userScreenName = userModel.ScreenName;  //This is the login name for the user log.
                // all other properties are now available is the userModel object two lines above.
            }
            _dbConnectorService.logModelCreator(userScreenName, "locked account: " + lockedAccountModel.ScreenName, "");
            //disable user account
            _dbConnectorService.immediateLockout(Id);
            return RedirectToAction(nameof(EditAccountDetails), new { Id = Id });
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Unlock(string? Id)
        {
            AppUserModel userModel = new AppUserModel();
            AppUserModel lockedAccountModel = _dbConnectorService.GetUserDetailsById(Id);
            string? userScreenName = "";
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                userModel = _dbConnectorService.GetUserDetailsById(userId);
                userScreenName = userModel.ScreenName;  //This is the login name for the user log.
                // all other propertiAes are now available is the userModel object two lines above.

            }
            _dbConnectorService.logModelCreator(userScreenName, "unlocked account: " + lockedAccountModel.ScreenName, "");
            //enable user account
            _dbConnectorService.disableLockout(Id);
            return RedirectToAction(nameof(EditAccountDetails), new { Id = Id });
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> ViewLogs(string? Id)
        {
            List<LogModel> logs = new List<LogModel>();
            logs = _dbConnectorService.GetLogs();
            logs.Reverse();
            return View(logs);
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]

        public async Task<IActionResult> SystemSettings()
        {
            SettingsModel settings = new SettingsModel();
            settings = _dbConnectorService.GetSystemSettings();
            //UpdateSystemSettings
            return View(settings);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> PostSystemSettings([Bind("Id, business_name, open_close_date, closing_user, open_close_on_date")] SettingsModel settingsIn)
        {
            SettingsModel updatedSettings = new SettingsModel();
            updatedSettings = _dbConnectorService.UpdateSystemSettings(settingsIn);

            return RedirectToAction(nameof(SystemSettings));
        }
        //Demo Reset
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> ResetTransactions()
        {
            ResetDataService resetService = new ResetDataService();
            resetService.ResetDataTransaction();
            return RedirectToAction(nameof(SystemSettings));
        }
        //Demo Reset
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> ResetAllData()
        {
            ResetDataService resetService = new ResetDataService();
            resetService.DemoDataResetAll();
            return RedirectToAction(nameof(SystemSettings));
        }
    }
}
