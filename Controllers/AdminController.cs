using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AdminController(ILogger<AdminController> logger, RoleManager<IdentityRole> roleManager)
        {
            this.logger = logger;
            this._roleManager = roleManager;
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
    }
}
