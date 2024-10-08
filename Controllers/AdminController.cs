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

        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ManageAccounts()
        {
            var appUsers = await _DbContext.Users.ToListAsync();
            return View(appUsers);
        }
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

        [HttpPost]
        public async Task<IActionResult> SendEmail([Bind("Id, Email, Subject, Message")] AppUserModel detailsIn)
        {
            AppUserModel appUserMessage = new AppUserModel();
            appUserMessage = detailsIn;
            await _emailSender.SendEmailAsync(appUserMessage.Email, appUserMessage.Subject, appUserMessage.Message);
            return RedirectToAction(nameof(ManageAccounts));
        }
        [HttpGet]
        public async Task<IActionResult> Lock(string? Id)
        {
            AppUserModel userModel = new AppUserModel();
            string? userScreeName = "";
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                userModel = _dbConnectorService.GetUserDetailsById(userId);
                userScreeName = userModel.ScreenName;  //This is the login name for the user log.
                // all other properties are now available is the userModel object two lines above.
            }

            //disable user account
            //_dbConnectorService.immediateLockout(Id, userScreenName);
            return RedirectToAction(nameof(EditAccountDetails), new { Id = Id });
        }
        [HttpGet]
        public async Task<IActionResult> Unlock(string? Id)
        {
            AppUserModel userModel = new AppUserModel();
            string? userScreeName = "";
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                userModel = _dbConnectorService.GetUserDetailsById(userId);
                userScreeName = userModel.ScreenName;  //This is the login name for the user log.
                // all other properties are now available is the userModel object two lines above.
            }

            //enable user account
            //_dbConnectorService.disableLockout(Id, userScreenName);
            return RedirectToAction(nameof(EditAccountDetails), new { Id = Id });
        }

        [HttpGet]
        public async Task<IActionResult> ViewLogs(string? Id)
        {
            List<LogModel> logs = new List<LogModel>();


            return View(logs);
        }
    }
}
