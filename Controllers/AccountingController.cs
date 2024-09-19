using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnAccount.Services;
using OnAccount.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace OnAccount.Controllers
{
    //to authorize multiple roles for every method in the class use like this: [Authorize(Roles = "Administrator,Accountant")]
    public class AccountingController : Controller
    {
        private readonly DbConnectorService _connectorService;
        public IEnumerable<SelectListItem> NormalSideOptionsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AccountTypeOptionsList { get; set; } = new List<SelectListItem>();
        
        private readonly UserService _userService;
        public AccountingController(DbConnectorService connectorService, UserService userService)
        {
            _connectorService = connectorService; // Database connection methods available here.
            this.NormalSideOptionsList = _connectorService.GetNormalSideOptions();
            this.AccountTypeOptionsList = _connectorService.GetAccountTypeOptions();
            _userService = userService; // This service provides access to the currently logged in users profile data.
        }

        //to authorize for a specific page. [Authorize(Roles = "Administrator,Manager,Accountant")] will -
        //allow all role types access to the return view through the Index method.
        [Authorize(Roles = "Administrator,Manager,Accountant")]
        public IActionResult Index()
        {
            //The manager and accountant roles will be directed here for their index.
            //The manager is routed to the management controller first and will have a link to use the accounting system.

            return View();
        }
    }
}
