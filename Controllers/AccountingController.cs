using Microsoft.AspNetCore.Mvc;
using oa.Services;
using oa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using System;

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
            List<AccountsModel> accountsModels = currentAccounts;
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
            accountModel.journal_id = _dbConnectorService.GetNextJournalId();
            AccountsModelJournal journalAccount = new AccountsModelJournal();
            journalAccount.journal_date = System.DateTime.Today;
            journalAccount.journal_id = accountModel.journal_id;
            journalAccount.accounts_list = currentAccounts;
            return View(journalAccount);
        }
        [HttpPost]
        [Route("api/journal")]
        public async Task<IActionResult> PostJournalEntry()
        {
            var uploadFileName = "";
            var formData = await Request.ReadFormAsync();
            var journalData = formData["journalData"].FirstOrDefault();
            if (journalData == null)
            {
                return BadRequest("Journal data missing.");
            }
            var journalEntry = JsonConvert.DeserializeObject<JournalEntry>(journalData);

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }
            foreach (var file in formData.Files)
            {
                var fileName = file.FileName;
                var fileExtension = Path.GetExtension(fileName);
                var newFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                var filePath = Path.Combine(uploadDirectory, newFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var transaction = journalEntry.Transactions.FirstOrDefault(t => t.TransactionUpload == fileName);
                if (transaction != null)
                {
                    transaction.TransactionUpload = filePath;
                    uploadFileName = newFileName;
                }
            }
            // loop handles infinite transactions and line items.
            for (int j = 0; j < journalEntry.Transactions.Count; j++)
            {
                for (int i = 0; i < journalEntry.Transactions[j].LineItems.Count; i++)
                {
                    TransactionModel transactionIn = new TransactionModel();
                    transactionIn.created_by = journalEntry.UserName;
                    transactionIn.journal_date = DateTime.Parse(journalEntry.JournalDate);
                    transactionIn.journal_description = journalEntry.JournalNotes;
                    transactionIn.status = journalEntry.JournalStatus;
                    transactionIn.journal_id = journalEntry.JournalId;
                    transactionIn.transaction_number = j;
                    transactionIn.transaction_date = journalEntry.Transactions[j].TransactionDate;
                    if (journalEntry.Transactions[j].LineItems[i].CrAccount == "unselected") { transactionIn.credit_account = 0; } else { transactionIn.credit_account = int.Parse(journalEntry.Transactions[j].LineItems[i].CrAccount); };
                    transactionIn.credit_amount = (double?)journalEntry.Transactions[j].LineItems[i].CrAmount;
                    if (journalEntry.Transactions[j].LineItems[i].DrAccount == "unselected") { transactionIn.debit_account = 0; } else { transactionIn.debit_account = int.Parse(journalEntry.Transactions[j].LineItems[i].DrAccount); };
                    transactionIn.debit_amount = (double?)journalEntry.Transactions[j].LineItems[i].DrAmount;
                    transactionIn.description = journalEntry.Transactions[j].TransactionDescription;
                    transactionIn.supporting_document = uploadFileName;
                    _dbConnectorService.AddTransaction(transactionIn);
                }
            }
            return RedirectToAction(nameof(ChartOfAccounts));
        }

        //All users can view accounts details pages
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewAccountDetails(string? id)
        {
            List<TransactionModel> currentTransactions = _dbConnectorService.GetAccountTransactions(id);

            ViewBag.AccountName = id +" - "+ _dbConnectorService.GetAccoutName(id);

            DateTime currentDate = DateTime.Now;
            ViewBag.Date = currentDate.ToString("dd-MM-yyyy");

            double totalDebitAmount = currentTransactions.Sum(t => t.debit_amount ?? 0);
            double totalCreditAmount = currentTransactions.Sum(t => t.credit_amount ?? 0);
            double accountBalance = _dbConnectorService.CalculateAccountBalance(id);

            ViewBag.TotalDebitAmount = totalDebitAmount;
            ViewBag.TotalCreditAmount = totalCreditAmount;
            ViewBag.AccountBalance = accountBalance; 
            return View(currentTransactions);
        }
        //All users can view accounts details pages
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> Details()
        {
            return View();
        }

        //All users can view accounts details pages
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> GeneralJournal()
        {
            List<TransactionModel> currentTransactions = _dbConnectorService.GetAllTransactions();

            return View(currentTransactions);
        }

    }
}
