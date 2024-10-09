using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using oa.Services;
using oa.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Principal;

namespace OnAccount.Controllers
{
    [BindProperties(SupportsGet = true)]
    public class AccountingController : Controller
    {
        private readonly DbConnectorService _dbConnectorService;        
        private readonly UserService _userService;
        List<AccountsModel> currentAccounts;
        AccountsModel accountModel;

        public AccountingController(DbConnectorService connectorService,
            UserService userService)
        {
            _dbConnectorService = connectorService;
            _userService = userService;
            currentAccounts = _dbConnectorService.GetChartOfAccounts();
            accountModel = new AccountsModel();
        }
        //All users can view the accounting home landing page
        [Authorize(Roles = "Administrator,Manager,Accountant")]
        public IActionResult Index()
        {
            return View();
        }
        //All users can view the chart of accounts
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ChartOfAccounts()
        {   
            List<AccountsModel> accountsModels = _dbConnectorService.GetChartOfAccounts();
            return View(accountsModels);
        }
        //Only administrators can add accounts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddAccount()
        {

            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            AccountsModel accountModel = new AccountsModel();
            accountModel.accounts_list = currentAccounts;
            return View(accountModel);
        }

        //Only administrators can add accounts
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveNewAccountDetails([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, transaction_1_date, transaction_1_dr, transaction_1_cr, transaction_2_date, transaction_2_dr, transaction_2_cr, transaction_dr_total, transaction_cr_total, accounts_list")] AccountsModel newaccountDetailsIn)
        {

            if (!ModelState.IsValid)
            {
                List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
                newaccountDetailsIn.accounts_list = currentAccounts;
                return View("AddAccount", newaccountDetailsIn);
            }

            AccountsModel newAccountModel = newaccountDetailsIn;
            newAccountModel = _dbConnectorService.CreateNewAccount(newAccountModel);
            return RedirectToAction(nameof(ChartOfAccounts));
        }
        //All users can view
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> EditAccount(string? Id)
        {
            AccountsModel account = _dbConnectorService.GetAccount(Id);
            AccountsModelEdit editAccount = new AccountsModelEdit();
            editAccount.id = account.id;
            editAccount.number = account.number;
            editAccount.name = account.name;
            editAccount.description = account.description;
            editAccount.type = account.type;
            editAccount.term = account.term;
            editAccount.statement_type = account.statement_type;
            editAccount.normal_side = account.normal_side;
            editAccount.created_by = account.created_by;
            editAccount.account_creation_date = account.account_creation_date;
            editAccount.starting_balance = account.starting_balance;
            editAccount.current_balance = account.current_balance;
            editAccount.sort_priority = account.sort_priority;
            editAccount.account_status = account.account_status;

            return View(editAccount);
        }
        //managers and accountants cannot edit accounts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateAccountDetails([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, transaction_1_date, transaction_1_dr, transaction_1_cr, transaction_2_date, transaction_2_dr, transaction_2_cr, transaction_dr_total, transaction_cr_total, accounts_list")] AccountsModelEdit accountDetailsIn)
        {
            if (!ModelState.IsValid)
            {
                List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
                AccountsModelEdit editAccount = new AccountsModelEdit();
                editAccount.id = accountDetailsIn.id;
                editAccount.number = accountDetailsIn.number;
                editAccount.name = accountDetailsIn.name;
                editAccount.description = accountDetailsIn.description;
                editAccount.type = accountDetailsIn.type;
                editAccount.term = accountDetailsIn.term;
                editAccount.statement_type = accountDetailsIn.statement_type;
                editAccount.normal_side = accountDetailsIn.normal_side;
                editAccount.created_by = accountDetailsIn.created_by;
                editAccount.account_creation_date = accountDetailsIn.account_creation_date;
                editAccount.starting_balance = accountDetailsIn.starting_balance;
                editAccount.current_balance = accountDetailsIn.current_balance;
                editAccount.sort_priority = accountDetailsIn.sort_priority;
                editAccount.account_status = accountDetailsIn.account_status;
                editAccount.accounts_list = currentAccounts;
                return View("EditAccount", editAccount);
            }

            AccountsModelEdit accountIn = accountDetailsIn;
            _dbConnectorService.UpdateExistingAccount(accountIn);
            return RedirectToAction(nameof(ChartOfAccounts));
        }
        // managers and users cannot disable accounts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DisableAccount()
        {
            List<AccountsModel> accountsModels = _dbConnectorService.GetChartOfAccounts();
            return View(accountsModels);
        }
        //All users can view jounal entries
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> AddJounalEntries()
        {
            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            AccountsModel accountModel = new AccountsModel();
            accountModel.accounts_list = currentAccounts;
            accountModel.nextJournalId = _dbConnectorService.GetNextJournalId();
            
            AccountsModelJournal journalAccount = new AccountsModelJournal();
            journalAccount.id = accountModel.id;
            journalAccount.created_by = accountModel.created_by;
            journalAccount.account_creation_date = accountModel.account_creation_date;
            journalAccount.total_adjustment = accountModel.total_adjustment;
            journalAccount.account_status = accountModel.account_status;
            journalAccount.transaction_1_date = accountModel.transaction_1_date;
            journalAccount.transaction_1_dr_account = accountModel.transaction_1_dr_account;
            journalAccount.transaction_1_dr = accountModel.transaction_1_dr;
            journalAccount.transaction_1_cr_account = accountModel.transaction_1_cr_account;
            journalAccount.transaction_1_cr = accountModel.transaction_1_cr;
            journalAccount.transaction_dr_total = accountModel.transaction_dr_total;
            journalAccount.transaction_cr_total = accountModel.transaction_cr_total;
            journalAccount.nextJournalId = accountModel.nextJournalId;
            List<AccountsModel> Accounts = _dbConnectorService.GetChartOfAccounts();
            journalAccount.accounts_list = Accounts;
            return View(journalAccount);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Accountant")]
        public async Task<IActionResult> SaveNewJounalEntry([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, transaction_1_date, transaction_1_dr, transaction_1_cr, transaction_2_date, transaction_2_dr, transaction_2_cr, transaction_dr_total, transaction_cr_total, accounts_list, transaction_1_cr_account, transaction_1_dr_account, transaction_1_description, nextJournalId")] AccountsModelJournal newJournalDetailsIn)
        {
/*            if (!ModelState.IsValid)
            {
                List<AccountsModel> Accounts = _dbConnectorService.GetChartOfAccounts();
                AccountsModel accountModel = newJournalDetailsIn;
                AccountsModelJournal journalAccount = new AccountsModelJournal();
                journalAccount.id = accountModel.id;
                journalAccount.created_by = accountModel.created_by;
                journalAccount.account_creation_date = accountModel.account_creation_date;
                journalAccount.total_adjustment = accountModel.total_adjustment;
                journalAccount.account_status = accountModel.account_status;
                journalAccount.transaction_1_date = accountModel.transaction_1_date;
                journalAccount.transaction_1_dr_account = accountModel.transaction_1_dr_account;
                journalAccount.transaction_1_dr = accountModel.transaction_1_dr;
                journalAccount.transaction_1_cr_account = accountModel.transaction_1_cr_account;
                journalAccount.transaction_1_cr = accountModel.transaction_1_cr;
                journalAccount.transaction_dr_total = accountModel.transaction_dr_total;
                journalAccount.transaction_cr_total = accountModel.transaction_cr_total;*//*
                newJournalDetailsIn.accounts_list = Accounts;
                return View("AddJounalEntries", newJournalDetailsIn);
            }*/


            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            AccountsModelJournal newJournal = newJournalDetailsIn;
            newJournal.accounts_list = currentAccounts;
            
            TransactionModel transaction = new TransactionModel();
            transaction.debit_account = int.Parse(newJournalDetailsIn.transaction_1_dr_account); 
            transaction.credit_account = int.Parse(newJournalDetailsIn.transaction_1_cr_account);
            transaction.debit_amount = double.Parse(newJournalDetailsIn.transaction_1_dr, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US")); 
            transaction.credit_amount = double.Parse(newJournalDetailsIn.transaction_1_cr, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"));
            transaction.description = newJournalDetailsIn.transaction_1_description;
            transaction.created_by = newJournalDetailsIn.created_by;
            transaction.transaction_date = System.DateTime.Now;
            transaction.journal_id = newJournalDetailsIn.nextJournalId;

            _dbConnectorService.AddTransaction(transaction);
            /*transaction.isOpening = newJournal.is_opening;*/

            return RedirectToAction(nameof(Index));
        }
        //All users can view accounts details pages
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewAccountDetails(string? id)
        {
            List<TransactionModel> currentTransactions = _dbConnectorService.GetAccountTransactions(id);

            ViewBag.AccountName = _dbConnectorService.GetAccoutName(id);
            DateTime currentDate = DateTime.Now;
            ViewBag.Date = currentDate.ToString("dd-MM-yyyy");

            double totalDebitAmount = currentTransactions.Sum(t => t.debit_amount ?? 0);
            double totalCreditAmount = currentTransactions.Sum(t => t.credit_amount ?? 0);
            double accountBalance = totalDebitAmount - totalCreditAmount;

            ViewBag.TotalDebitAmount = totalDebitAmount;
            ViewBag.TotalCreditAmount = totalCreditAmount;
            ViewBag.AccountBalance = accountBalance; 
            return View(currentTransactions);
        }

    }
}
