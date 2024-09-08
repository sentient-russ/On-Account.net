using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OnAccount.Areas.Identity.Data;
using OnAccount.Models;
using System.ComponentModel;
using System.Threading.Tasks;

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


        public AdminController(ILogger<AdminController> logger, 
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

        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {

            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ManageAccounts(IdentityRole model)
        {
            var users = _userManager.Users;

            return View(users);
        }

    }
}
