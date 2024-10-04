using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using oa.Services;
using oa.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace OnAccount.Controllers
{

    [BindProperties(SupportsGet = true)]
    public class AccountingController : Controller
    {
        private readonly DbConnectorService _dbConnectorService;        
        private readonly UserService _userService;

        public AccountingController(DbConnectorService connectorService,
            UserService userService)
        {
            _dbConnectorService = connectorService; // Database connection methods available here.
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

        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ChartOfAccounts()
        {   
            List<AccountsModel> accountsModels = _dbConnectorService.GetChartOfAccounts();
            return View(accountsModels);
        }

        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> AddAccount()
        {
            AccountsModel accountModel = new AccountsModel();
            return View(accountModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveNewAccountDetails([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance")] AccountsModel newaccountDetailsIn)
        {
            AccountsModel newAccountModel = new AccountsModel();
            newAccountModel = _dbConnectorService.CreateNewAccount(newaccountDetailsIn);
            //add code here to add the first accounts journal entry.
            return RedirectToAction(nameof(ChartOfAccounts));
        }


        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> EditAccount()
        {
            List<AccountsModel> accountsModels = _dbConnectorService.GetChartOfAccounts();
            return View(accountsModels);
        }
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DisableAccount()
        {
            List<AccountsModel> accountsModels = _dbConnectorService.GetChartOfAccounts();
            return View(accountsModels);
        }
    }
}
