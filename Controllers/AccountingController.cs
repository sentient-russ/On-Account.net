using Microsoft.AspNetCore.Mvc;
using oa.Services;
using oa.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.UI.Services;
using oa.Areas.Identity.Data;
using oa.Migrations;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace OnAccount.Controllers
{
    [BindProperties(SupportsGet = true)]
    public class AccountingController : Controller
    {
        private readonly DbConnectorService _dbConnectorService;
        private readonly UserService _userService;
        List<AccountsModel> currentAccounts;
        AccountsModel accountModel;
        private readonly IEmailSender _emailSender;
        public DateTime today = DateTime.Today;
        public string? todayStr = "";
        private readonly UserManager<AppUser> _userManager;

        public AccountingController(DbConnectorService connectorService,
            UserService userService, IEmailSender emailSender, UserManager<AppUser> userManager)
        {
            _dbConnectorService = connectorService;
            _userService = userService;
            currentAccounts = _dbConnectorService.GetChartOfAccounts().OrderBy(a => a.number).ToList();
            accountModel = new AccountsModel();
            this._emailSender = emailSender;
            todayStr = today.ToString("MM-dd-yyyy");
            _userManager = userManager;
        }
        //All users can view the accounting home landing page
        [Authorize(Roles = "Administrator,Manager,Accountant")]
        public IActionResult Index()
        {
            //leaving this as example on obtaining users details from identity claims.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            AppUserModel userDetails = _dbConnectorService.GetUserDetailsById(userId);

            ChartingDataService chartingService = new ChartingDataService();
            DashboardBundleModel dashboardBundleModel;
            CurrentRaitoModel currentRaitoModel;
            ReturnOnAssetsModel returnOnAssetsModel;
            ReturnOnEquityModel returnOnEquityModel;
            QuickRatioModel quickRatioModel;
            currentRaitoModel = new CurrentRaitoModel();
            currentRaitoModel.current_assets_balance = chartingService.GetAccountTypeTotalBalance("Asset", "Short");
            currentRaitoModel.current_liabilities_balance = chartingService.GetAccountTypeTotalBalance("Liability", "Short");
            returnOnAssetsModel = new ReturnOnAssetsModel();
            returnOnAssetsModel.revenues_balance = chartingService.GetAccountTypeTotalBalance("Revenue");
            returnOnAssetsModel.expenses_balance = chartingService.GetAccountTypeTotalBalance("Expense");
            returnOnAssetsModel.assets_balance = chartingService.GetAccountTypeTotalBalance("Asset");
            returnOnEquityModel = new ReturnOnEquityModel();
            returnOnEquityModel.equity_balance = chartingService.GetAccountTypeTotalBalance("Equity");
            quickRatioModel = new QuickRatioModel();
            List<string> inventoryIdList = new List<string>();
            inventoryIdList.Add("115"); // Office Equipment
            inventoryIdList.Add("140"); // Merchandise Inventory

            quickRatioModel.inventory_balance = chartingService.GetAccountIdsTotalBalane(inventoryIdList);

            dashboardBundleModel = 
                new DashboardBundleModel(
                currentRaitoModel, 
                returnOnAssetsModel,
                returnOnEquityModel,
                quickRatioModel
                );

            ViewBag.pendingExists = _dbConnectorService.PendingTransactionExists();

            return View(dashboardBundleModel);
        }
        //All users can view the chart of accounts
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ChartOfAccounts()
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<AccountsModel> accountsModels = currentAccounts;
            return View(accountsModels);
        }
        //Only administrators can add accounts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddAccount()
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            AccountsModel accountModel = new AccountsModel();
            accountModel.accounts_list = currentAccounts;
            return View(accountModel);
        }

        //Only administrators can add accounts
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveNewAccountDetails([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, transaction_1_date, transaction_1_dr, transaction_1_cr, transaction_2_date, transaction_2_dr, transaction_2_cr, transaction_dr_total, transaction_cr_total, accounts_list, comments")] AccountsModel newaccountDetailsIn)
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
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            int id = Int32.Parse(Id);
            string accountNumber = _dbConnectorService.GetAccoutNumber(id);
            AccountsModel account = _dbConnectorService.GetAccount(accountNumber);
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
            editAccount.comments = account.comments;

            return View(editAccount);
        }
        //managers and accountants cannot edit accounts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateAccountDetails([Bind("id, name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, transaction_1_date, transaction_1_dr, transaction_1_cr, transaction_2_date, transaction_2_dr, transaction_2_cr, transaction_dr_total, transaction_cr_total, accounts_list, comments")] AccountsModelEdit accountDetailsIn)
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
                editAccount.comments = accountDetailsIn.comments;
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

        //All users can view jounal entries
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        [HttpPost]
        [Route("api/journal")]
        public async Task<IActionResult> PostJournalEntry()
        {
            var formData = await Request.ReadFormAsync();
            var journalData = formData["journalData"].FirstOrDefault();

            if (string.IsNullOrEmpty(journalData))
            {
                return BadRequest("Journal data is empty or null.");
            }

            JournalEntry journalEntry;
            try
            {
                journalEntry = JsonConvert.DeserializeObject<JournalEntry>(journalData);
            }
            catch (System.Text.Json.JsonException ex)
            {
                return BadRequest($"Invalid JSON data: {ex.Message}");
            }

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploaded_docs");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var transactionFileUploads = new Dictionary<int, List<string>>();
            int indeX = 0;
            foreach (var file in formData.Files)
            {
                var fileName = file.FileName;
                var fileExtension = Path.GetExtension(fileName);
                var newFileName = $"{indeX}_{journalEntry.JournalId}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                var filePath = Path.Combine(uploadDirectory, newFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _dbConnectorService.AddSupportingDocs(journalEntry.JournalId, newFileName);
                indeX = indeX + 1;
            }

            //orriginally built for each journal entry to handle multipe transactions.  Leaving for that as a potential future upgrade
            for (int j = 0; j < journalEntry.Transactions.Count; j++)
            {

                for (int i = 0; i < journalEntry.Transactions[j].LineItems.Count; i++)
                {
                    string supportingDocFound = "false";
                    if (indeX > 0)
                    {
                        supportingDocFound = "true";
                    }
                    var transactionIn = new TransactionModel
                    {
                        created_by = journalEntry.UserName,
                        journal_date = DateTime.Parse(journalEntry.JournalDate),
                        journal_description = journalEntry.JournalNotes,
                        status = journalEntry.JournalStatus,
                        journal_id = journalEntry.JournalId,
                        transaction_number = j,
                        transaction_date = journalEntry.Transactions[j].TransactionDate,
                        credit_account = journalEntry.Transactions[j].LineItems[i].CrAccount == "unselected" ? 0 : int.Parse(journalEntry.Transactions[j].LineItems[i].CrAccount),
                        credit_amount = (double)journalEntry.Transactions[j].LineItems[i].CrAmount,
                        debit_account = journalEntry.Transactions[j].LineItems[i].DrAccount == "unselected" ? 0 : int.Parse(journalEntry.Transactions[j].LineItems[i].DrAccount),
                        debit_amount = (double)journalEntry.Transactions[j].LineItems[i].DrAmount,
                        description = journalEntry.Transactions[j].TransactionDescription,
                        supporting_document = supportingDocFound,
                        is_adjusting = journalEntry.is_adjusting,

                    };

                    _dbConnectorService.AddTransaction(transactionIn);
                }
            }

            List<string> managerEmails = _dbConnectorService.GetManagerEmails();
            for (int i = 0; i < managerEmails.Count; i++)
            {
                string subject = $"On-Account notification: JID: {journalEntry.JournalId}";
                string message = $"A new journal entry has been submitted. JID: {journalEntry.JournalId}";
                await _emailSender.SendEmailAsync(managerEmails[i], subject, "");
            }

            return Ok(new { message = "Journal data received successfully", journalData = journalData });
        }

        //All users can view accounts details pages
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewAccountDetails(string? id)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<TransactionModel> currentTransactions = _dbConnectorService.GetAccountTransactions(id);
            for (var i = currentTransactions.Count - 1; 0 < i; i--)
            {
                if (currentTransactions[i].status == "Pending")
                {
                    currentTransactions.RemoveAt(i);
                }
                else if (currentTransactions[i].status == "Denied")
                {
                    currentTransactions.RemoveAt(i);
                }
            }
            ViewBag.AccountName = id + " - " + _dbConnectorService.GetAccoutName(id);
            ViewBag.AccountNumber = id;
            DateTime currentDate = DateTime.Now;
            ViewBag.Date = currentDate.ToString("MM-dd-yyyy");
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
        public async Task<IActionResult> GeneralJournal(string? Id = "1", string status = "All", string? messageIn = "", string? JID = "", List<string>? JIDListIn = null)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            List<TransactionModel> currentTransactions = new List<TransactionModel>();
            var journalsPerPage = 10;
            double totalPages = 0.0;

            if (JID != "")
            {
                totalPages = 1;
                currentTransactions = _dbConnectorService.GetAccountTransactionsByJournalNumber(JID);
            }
            // handles the event where a list of journal entries need to be closed to close out the accounting year
            else if (JIDListIn.Count != 0)
            {
                for (int i = 0; i < JIDListIn.Count; i++)
                {
                    List<TransactionModel> nextJournalTransactionsList = _dbConnectorService.GetAccountTransactionsByJournalNumber(JIDListIn[i]);
                    for (int j = 0; j < nextJournalTransactionsList.Count; j++)
                    {
                        currentTransactions.Add(nextJournalTransactionsList[j]);
                    }
                }
                totalPages = (double)currentTransactions.Count / (double)journalsPerPage;
                if (totalPages % 1 != 0)
                {
                    totalPages = (int)totalPages + 1;
                }
            }
            else if (status == "All")
            {
                var numberJournalsWithSatus = _dbConnectorService.GetNumJournalsStatus(status);
                //trim list to current pages transactions                
                totalPages = (double)numberJournalsWithSatus / (double)journalsPerPage;
                //add a page if a partial trailing page is required
                if (totalPages % 1 != 0)
                {
                    totalPages = (int)totalPages + 1;
                }
                // determine first and last journal number to send to the view
                var endingJournalNumber = (Int32.Parse(Id) * journalsPerPage);
                var startingJournalNumber = (endingJournalNumber - journalsPerPage) - 1;
                currentTransactions = _dbConnectorService.GetAllTransactionsByJournalRange(startingJournalNumber, endingJournalNumber);
            }
            else
            {
                //trim list to current pages transactions
                var numberJournalsWithSatus = _dbConnectorService.GetNumJournalsStatus(status);
                if (numberJournalsWithSatus == 0)
                {
                    //This is needed for when no transactions exist with a specified status to avoid devide by 0 error.
                    //An error woud be appropriate.
                    return RedirectToAction(nameof(GeneralJournal), new { messageIn = "No transactions exist with that status." });
                }
                else
                {
                    totalPages = (double)numberJournalsWithSatus / (double)journalsPerPage;
                }
                //calc number of pages
                if (totalPages % 1 != 0)
                {
                    totalPages = (int)totalPages + 1;
                }
                else if (totalPages % 1 == 0)
                {
                    totalPages = totalPages;
                }
                else if (totalPages <= 0)
                {
                    totalPages = 1;
                }
                else
                {
                    totalPages = (numberJournalsWithSatus / journalsPerPage) + 1;
                }
                var endingJournalNumber = (Int32.Parse(Id) * journalsPerPage);
                var startingJournalNumber = (endingJournalNumber - journalsPerPage);

                currentTransactions = _dbConnectorService.GetAllTransactionsByJournalStatus(startingJournalNumber, endingJournalNumber - 1, status);
            }

            for (var i = 0; i < currentTransactions.Count; i++)
            {
                if (currentTransactions[i].debit_amount == 0)
                {
                    currentTransactions[i].debit_amount = null;
                }
                else if (currentTransactions[i].credit_amount == 0)
                {
                    currentTransactions[i].credit_amount = null;
                }
            }
            for (int i = 0; i < currentTransactions.Count; i++)
            {
                currentTransactions[i].cr_description = currentAccounts
                    .Where(account => account.number == currentTransactions[i].credit_account)
                    .Select(account => account.name)
                    .FirstOrDefault();
                if (currentTransactions[i].credit_account != 0)
                {
                    currentTransactions[i].cr_description = currentTransactions[i].credit_account + " - " + currentTransactions[i].cr_description;
                }
                else
                {
                    currentTransactions[i].cr_description = null;
                }
                currentTransactions[i].dr_description = currentAccounts
                    .Where(account => account.number == currentTransactions[i].debit_account)
                    .Select(account => account.name)
                    .FirstOrDefault();
                if (currentTransactions[i].debit_account != 0)
                {
                    currentTransactions[i].dr_description = currentTransactions[i].debit_account + " - " + currentTransactions[i].dr_description;
                }
                else
                {
                    currentTransactions[i].dr_description = null;
                }
            }
            ViewBag.currentPage = Int32.Parse(Id);
            ViewBag.totalPages = (int)totalPages;
            ViewBag.previousPage = Int32.Parse(Id) - 1;
            if ((int)totalPages > Int32.Parse(Id))
            {
                ViewBag.nextPage = Int32.Parse(Id) + 1;
            }
            else
            {
                ViewBag.nextPage = 0;
            }
            ViewBag.recordCount = currentTransactions.Count();
            ViewBag.lastStatus = status;
            ViewBag.JournalFocusMessage = messageIn;
            return View(currentTransactions);
        }

        //All users can view journal details
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewJournalDetails(string? id)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            JournalBundle infoBundle = new JournalBundle();
            infoBundle.transactions_list = _dbConnectorService.GetAccountTransactionsByJournalNumber(id);
            infoBundle.journal_id = Int32.Parse(id);
            infoBundle.created_by = infoBundle.transactions_list[0].created_by;
            infoBundle.status = infoBundle.transactions_list[0].status;
            infoBundle.journal_date = infoBundle.transactions_list[0].journal_date;
            infoBundle.journal_description = infoBundle.transactions_list[0].journal_description;
            infoBundle.supporting_docs_list = _dbConnectorService.GetSupportingDocuments(Int32.Parse(id));
            infoBundle.is_adjusting = infoBundle.transactions_list[0].is_adjusting;

            for (int i = 0; i < infoBundle.transactions_list.Count; i++)
            {
                infoBundle.transactions_list[i].cr_description = currentAccounts
                    .Where(account => account.number == infoBundle.transactions_list[i].credit_account)
                    .Select(account => account.name)
                    .FirstOrDefault();
                if (infoBundle.transactions_list[i].credit_account != 0)
                {
                    infoBundle.transactions_list[i].cr_description = infoBundle.transactions_list[i].credit_account + " - " + infoBundle.transactions_list[i].cr_description;
                }
                else
                {
                    infoBundle.transactions_list[i].cr_description = null;
                }

                infoBundle.transactions_list[i].dr_description = currentAccounts
                    .Where(account => account.number == infoBundle.transactions_list[i].debit_account)
                    .Select(account => account.name)
                    .FirstOrDefault();

                if (infoBundle.transactions_list[i].debit_account != 0)
                {
                    infoBundle.transactions_list[i].dr_description = infoBundle.transactions_list[i].debit_account + " - " + infoBundle.transactions_list[i].dr_description;
                }
                else
                {
                    infoBundle.transactions_list[i].dr_description = null;
                }
            }
            double balance = 0;
            //Just add one side of the T account for the total journal amount
            for (int i = 0; i < infoBundle.transactions_list.Count; i++)
            {
                if (infoBundle.transactions_list[i].debit_amount >= 1)
                {
                    balance += (double)infoBundle.transactions_list[i].debit_amount;
                }
            }
            infoBundle.total_adjustment = balance;
            return View(infoBundle);
        }
        //Only the manager can approve or deny a transaction.
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DenyJournal(string? id)
        {

            _dbConnectorService.UpdateTransactionStatus(id, "Denied");
            // need log update here
            return RedirectToAction(nameof(GeneralJournal));
        }

        //Denial of journal with comment
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<JsonResult> DenyJournalWithCommentAsync(string? id, string? commenter, string? comment)
        {

            string formatted_comment = $"Denied by: {commenter}: {comment}";
            _dbConnectorService.UpdateJournalNotes(id, formatted_comment);
            // need log update here
            return Json(true);
        }

        //Only the manager can approve or deny a transaction.
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveJournal(string? id)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            _dbConnectorService.UpdateTransactionStatus(id, "Approved");
            List<TransactionModel> listOfTransactions = _dbConnectorService.GetAccountTransactionsByJournalNumber(id);
            List<int?> listOfAccount = new List<int?>();
            for (int i = 0; i < listOfTransactions.Count(); i++)
            {
                if (listOfTransactions[i].debit_account != 0)
                {
                    listOfAccount.Add(listOfTransactions[i].debit_account);
                }
                else if (listOfTransactions[i].credit_account != 0)
                {
                    listOfAccount.Add(listOfTransactions[i].credit_account);
                }
            }

            for (int i = 0; i < listOfAccount.Count(); i++)
            {
                _dbConnectorService.CalculateAccountBalance(listOfAccount[i].ToString());
            }
            // need log update here
            return RedirectToAction(nameof(GeneralJournal), new { status = "Pending" });
        }

        // Viewable and usable by ANY logged-in user/role
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> EmailAdmin(string? id)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;

            List<String> administrativeEmails = _dbConnectorService.GetAdministrativeEmails();

            AppUser emailBundle = new AppUser();

            emailBundle.AdminManagerEmails = administrativeEmails;

            return View(emailBundle);
        }
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        [HttpPost]
        public async Task<IActionResult> SendEmailAdmin([Bind("Id, Email, Subject, Message")] AppUserModel detailsIn)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            AppUserModel appUserMessage = new AppUserModel();
            appUserMessage = detailsIn;
            // Task runs in a standalone discard while the redirection takes place. (No more await)
            _ = Task.Run(() => _emailSender.SendEmailAsync(appUserMessage.Email, appUserMessage.Subject, appUserMessage.Message));
            return RedirectToAction("Index", "Home");
        }

        //grabs all logs that are relative to a specific account in the chart of accounts for the view
        [Authorize(Roles = "Manager, Administrator")]

        public async Task<IActionResult> viewAccountLogs(string? id)
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            AccountsModel currentAccount = _dbConnectorService.GetAccount(id);
            ViewBag.Accountname = currentAccount.name;
            string accountName = currentAccount.name;
            List<LogModel> logs = _dbConnectorService.GetAccountLogs(accountName);
            logs.Reverse();
            return View(logs);
        }

        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> viewTrialBalance(string? dateIn = "", string? includeAdjusting = "false")
        {
            string? fromDate = "";
            string? toDate = dateIn;
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            if (includeAdjusting == "true")
            {
                ViewBag.isAdjusting = "()";
            }

            DateTime lastClosingDate = (DateTime)systemSettings.open_close_date;
            ViewBag.lastClosingDate = lastClosingDate.ToString("yyyy-MM-dd");
            if (fromDate == "") { fromDate = lastClosingDate.ToString("MM-dd-yyyy"); }
            if (toDate == "") { toDate = lastClosingDate.ToString("MM-dd-yyyy"); }

            if (toDate != "")
            {
                DateTime titleDateObj = DateTime.Parse(toDate);
                ViewBag.titleDate = titleDateObj.ToString("MM-dd-yyyy");
                ViewBag.asOfDate = titleDateObj.ToString("yyyy-MM-dd");
            }

            SettingsModel settings = _dbConnectorService.GetSystemSettings();
            DateTime currentDate = DateTime.Now;
            var message = "";
            string viewInputDate = "";

            if (string.IsNullOrEmpty(dateIn) || dateIn == fromDate)
            {
                fromDate = settings.open_close_date.Value.ToString("MM-dd-yyyy");
                viewInputDate = settings.open_close_date.Value.ToString("yyyy-MM-dd");
                message = $"Please select a valid date after {settings.open_close_date.Value.ToString("MM-dd-yyyy")} and before {currentDate.ToString("MM-dd-yyyy")}. Note: All pending transactions must be 'Denied' or 'Approved' for the selected range.";
            }
            else
            {
                fromDate = settings.open_close_date.Value.ToString("MM-dd-yyyy");
                viewInputDate = DateTime.Parse(dateIn).ToString("yyyy-MM-dd");
            }

            // Check for pending transactions in the period
            bool pendingTransactionsFound = _dbConnectorService.CheckForPendingByDateRange(toDate);
            if (pendingTransactionsFound)
            {
                return RedirectToAction(nameof(GeneralJournal), new
                {
                    status = "Pending",
                    messageIn = "Pending transactions were found in the requested date range which must be Approved or Denied before a trial balance can be created."
                });
            }

            string retainedEarningsAccountNum = "290";
            _dbConnectorService.CalculateAccountBalance(retainedEarningsAccountNum);
            // Start date
            bool includeAdjustingBool = bool.Parse(includeAdjusting);
            List<AccountsModel> listOfAccounts = new List<AccountsModel>();
            listOfAccounts = _dbConnectorService.GetNonZeroAccounts();
            // Iterate over the non-zero accounts to update balances based on date range
            foreach (AccountsModel account in listOfAccounts)
            {
                if (account.type == "Expense" || account.type == "Revenue")
                {
                    account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjustingBool);
                }
                else
                {
                    account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, systemSettings.system_start_date.Value.ToString("MM-dd-yyyy"), toDate, includeAdjustingBool);
                }
            }
            _dbConnectorService.CalculateAccountBalance(retainedEarningsAccountNum);
            List<TrialBalanceModel> trialBalanceModels = new List<TrialBalanceModel>();
            for (int i = 0; i < listOfAccounts.Count(); i++)
            {
                TrialBalanceModel trialBalanceTemp = new TrialBalanceModel();
                if (listOfAccounts[i].current_balance != 0)
                {
                    if (listOfAccounts[i].normal_side.Equals("Debit"))
                    {
                        trialBalanceTemp.accountname = listOfAccounts[i].number + " - " + listOfAccounts[i].name;
                        trialBalanceTemp.debit = (double)listOfAccounts[i].current_balance;
                    }
                    if (listOfAccounts[i].normal_side.Equals("Credit"))
                    {
                        trialBalanceTemp.accountname = listOfAccounts[i].number + " - " + listOfAccounts[i].name;
                        trialBalanceTemp.credit = (double)listOfAccounts[i].current_balance;
                    }
                    trialBalanceModels.Add(trialBalanceTemp);
                }
            }
            //ViewBag.isAdjusting = includeAdjusting;
            ViewBag.InputDate = viewInputDate;
            ViewBag.AsOfDate = toDate;
            ViewBag.RecentOpeningClosingDate = settings.open_close_date.Value.ToString("MM-dd-yyyy");
            
            if (includeAdjusting == "true")
            {
                ViewBag.isAdjusting = "true";
            }
            if(trialBalanceModels.Count != 0)
            {
                ViewBag.Message = "";
            } else
            {
                fromDate = settings.open_close_date.Value.ToString("MM-dd-yyyy");
                viewInputDate = settings.open_close_date.Value.ToString("yyyy-MM-dd");
                message = $"Please select a valid date after {settings.open_close_date.Value.ToString("MM-dd-yyyy")} and before {currentDate.ToString("MM-dd-yyyy")}. Note: All pending transactions must be 'Denied' or 'Approved' for the selected range.";
                ViewBag.Message = message;
            }
            var sortedTrialBalanceModels = trialBalanceModels
                .OrderBy(t => t.credit)
                .ThenBy(t => t.accountname)
                .ToList();
            return View(sortedTrialBalanceModels);
        }

        //view for the income statement
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> viewIncomeStatement(string fromDate = "", string? toDate = "")
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            DateTime lastClosingDate = (DateTime)systemSettings.open_close_date;
            ViewBag.lastClosingDate = lastClosingDate.ToString("yyyy-MM-dd");
            if (fromDate == "") { fromDate = lastClosingDate.ToString("MM-dd-yyyy"); }
            if (toDate == "") { toDate = DateTime.Now.ToString("MM-dd-yyyy"); }
            ViewBag.asOfDate = toDate; if (toDate != "")
            {
                DateTime titleDateObj = DateTime.Parse(toDate);
                ViewBag.titleDate = titleDateObj.ToString("MM-dd-yyyy");
                ViewBag.asOfDate = titleDateObj.ToString("yyyy-MM-dd");
            }
            IncomeStatementBundle incomeBundle = new IncomeStatementBundle();
            List<AccountsModel> allAccounts = _dbConnectorService.GetChartOfAccounts();
            bool includeAdjusting = true;

            foreach (AccountsModel account in allAccounts)
            {
                if (account.type == "Revenue" && account.current_balance != 0)
                {
                    account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.RevenueAccountsList.Add(account);
                    incomeBundle.RevenueAccountsTotal += (double)account.current_balance;
                }
                if (account.type == "Expense" && account.current_balance != 0)
                {
                    account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.ExpenseAccountsList.Add(account);
                    incomeBundle.ExxpenseAccountsTotal += (double)account.current_balance;
                }
            }
            incomeBundle.Net = incomeBundle.RevenueAccountsTotal - incomeBundle.ExxpenseAccountsTotal;
            return View(incomeBundle);
        }
        //view for the balance sheet
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> ViewBalanceSheet(string fromDate = "", string? toDate = "")
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            DateTime lastClosingDate = (DateTime)systemSettings.system_start_date;
            ViewBag.lastClosingDate = lastClosingDate.ToString("yyyy-MM-dd");
            if (fromDate == "") { fromDate = lastClosingDate.ToString("MM-dd-yyyy"); }
            if (toDate == "") { toDate = DateTime.Now.ToString("MM-dd-yyyy"); }

            if (toDate != "")
            {
                DateTime titleDateObj = DateTime.Parse(toDate);
                ViewBag.titleDate = titleDateObj.ToString("MM-dd-yyyy");
                ViewBag.asOfDate = titleDateObj.ToString("yyyy-MM-dd");
            }

            BalanceSheetBundle balanceBundle = new BalanceSheetBundle();
            List<AccountsModel> allAccounts = _dbConnectorService.GetChartOfAccounts();
            bool includeAdjusting = true;
            foreach (AccountsModel account in allAccounts)
            {
                if (account.type == "Asset" && account.term == "Short" && account.current_balance != 0)
                {
                    string? normal_side = _dbConnectorService.GetAccountNormalSideByNumber(account.number.ToString());
                    if (normal_side == "Debit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                        if(account.current_balance > 0)
                        {
                            balanceBundle.ShortTermAssets.Add(account);
                            balanceBundle.ShortTermAssetsTotal += (double)account.current_balance;
                        }
                    }
                    else if (normal_side == "Credit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting) * -1;
                        if(account.current_balance > 0)
                        {
                            balanceBundle.ShortTermAssets.Add(account);
                            balanceBundle.ShortTermAssetsTotal -= (double)account.current_balance;
                        }
                    }
                }
                else if (account.type == "Asset" && account.term == "Long" && account.current_balance != 0)
                {
                    string? normal_side = _dbConnectorService.GetAccountNormalSideByNumber(account.number.ToString());
                    if (normal_side == "Debit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                        if(account.current_balance > 0)
                        {
                            balanceBundle.LongTermAssets.Add(account);
                            balanceBundle.LongTermAssetsTotal += (double)account.current_balance;
                        }

                    }
                    else if (normal_side == "Credit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting) * -1;
                        if(account.current_balance > 0)
                        {
                            balanceBundle.LongTermAssets.Add(account);
                            balanceBundle.LongTermAssetsTotal += (double)account.current_balance;
                        }

                    }
                }
                else if (account.type == "Liability" && account.term == "Short" && account.current_balance != 0)
                {
                    string? normal_side = _dbConnectorService.GetAccountNormalSideByNumber(account.number.ToString());
                    if (normal_side == "Credit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                        if (account.current_balance > 0)
                        {
                            balanceBundle.ShortTermLiabilities.Add(account);
                            balanceBundle.ShortTermLiabilitiesTotal += (double)account.current_balance;
                        }
                    }
                    else if (normal_side == "Debit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting) * -1;
                        if (account.current_balance > 0)
                        {
                            balanceBundle.ShortTermLiabilities.Add(account);
                            balanceBundle.ShortTermLiabilitiesTotal += (double)account.current_balance;
                        }

                    }
                }
                else if (account.type == "Liability" && account.term == "Long" && account.current_balance != 0)
                {
                    string? normal_side = _dbConnectorService.GetAccountNormalSideByNumber(account.number.ToString());
                    if (normal_side == "Credit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                        if (account.current_balance > 0)
                        {
                            balanceBundle.LongTermLiabilities.Add(account);
                            balanceBundle.LongTermLiabilitiesTotal += (double)account.current_balance;
                        }
                    }
                    else if (normal_side == "Debit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting) * -1;
                        if (account.current_balance > 0)
                        {
                            balanceBundle.LongTermLiabilities.Add(account);
                            balanceBundle.LongTermLiabilitiesTotal += (double)account.current_balance;
                        }
                    }
                }
                else if (account.type == "Equity")
                {
                    string? normal_side = _dbConnectorService.GetAccountNormalSideByNumber(account.number.ToString());
                    if (normal_side == "Credit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting);
                        if (account.number == 290)
                        {
                            IncomeStatementBundle incomeBundle = new IncomeStatementBundle();
                            foreach (AccountsModel accnt in allAccounts)
                            {
                                if (accnt.type == "Revenue" && accnt.current_balance != 0)
                                {
                                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                                    incomeBundle.RevenueAccountsList.Add(accnt);
                                    incomeBundle.RevenueAccountsTotal += (double)accnt.current_balance;
                                }
                                if (accnt.type == "Expense" && accnt.current_balance != 0)
                                {
                                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                                    incomeBundle.ExpenseAccountsList.Add(accnt);
                                    incomeBundle.ExxpenseAccountsTotal += (double)accnt.current_balance;
                                }
                            }
                            account.current_balance = account.current_balance + (decimal)(incomeBundle.RevenueAccountsTotal - incomeBundle.ExxpenseAccountsTotal);
                        }

                        balanceBundle.Equities.Add(account);
                        balanceBundle.EquityTotal += (double)account.current_balance;
                    }
                    else if (normal_side == "Debit")
                    {
                        account.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(account.number, fromDate, toDate, includeAdjusting) * -1;
                        balanceBundle.Equities.Add(account);
                        balanceBundle.EquityTotal += (double)account.current_balance;
                    }
                }
                balanceBundle.TotalLiabilitiesStockHolderEquity = balanceBundle.ShortTermLiabilitiesTotal + balanceBundle.LongTermLiabilitiesTotal + balanceBundle.EquityTotal;
            }
            return View(balanceBundle);
        }

        //view for the Owner's equity
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> viewOwnersEquity(string fromDate = "", string? toDate = "")
        {
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            ViewBag.businessName = systemSettings.business_name;
            DateTime lastClosingDate = (DateTime)systemSettings.open_close_date;
            ViewBag.lastClosingDate = lastClosingDate.ToString("yyyy-MM-dd");
            if (fromDate == "") { fromDate = lastClosingDate.ToString("MM-dd-yyyy"); }
            if (toDate == "") { toDate = DateTime.Now.ToString("MM-dd-yyyy"); }
            if (toDate != "")
            {
                DateTime titleDateObj = DateTime.Parse(toDate);
                ViewBag.titleDate = titleDateObj.ToString("MM-dd-yyyy");
                ViewBag.asOfDate = titleDateObj.ToString("yyyy-MM-dd");
            }
            EquityBundle equityBundle = new EquityBundle();
            List<AccountsModel> allAccounts = _dbConnectorService.GetChartOfAccounts();
            bool includeAdjusting = true;

            // calc running net income
            IncomeStatementBundle incomeBundle = new IncomeStatementBundle();
            foreach (AccountsModel accnt in allAccounts)
            {
                if (accnt.type == "Revenue" && accnt.current_balance != 0)
                {
                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.RevenueAccountsList.Add(accnt);
                    incomeBundle.RevenueAccountsTotal += (double)accnt.current_balance;
                }
                if (accnt.type == "Expense" && accnt.current_balance != 0)
                {
                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.ExpenseAccountsList.Add(accnt);
                    incomeBundle.ExxpenseAccountsTotal += (double)accnt.current_balance;
                }
            }
            equityBundle.BeginningRetainedEarnings = (double)_dbConnectorService.GetAccountBalanceForApprovedByDateRange(290, fromDate, toDate, includeAdjusting);
            equityBundle.RunningNetIncome = incomeBundle.RevenueAccountsTotal - incomeBundle.ExxpenseAccountsTotal;
            equityBundle.RunningDividends = (double)_dbConnectorService.GetAccountBalanceForApprovedByDateRange(295, fromDate, toDate, includeAdjusting);
            equityBundle.EndRetainedEarnings = equityBundle.BeginningRetainedEarnings + equityBundle.RunningNetIncome - equityBundle.RunningDividends;
            return View(equityBundle);
        }

        //end point to process the closing entry and return the journalId.
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateClosingEntry()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDetails = _dbConnectorService.GetUserDetailsById(userId);
            var openCloseDate = Request.Form["open_close_date"].ToString();
            DateTime newClosingDate = DateTime.Parse(openCloseDate);
            //check for and handle pending in range condition
            List<string> pendingIds = _dbConnectorService.GetPeriodPendingJournalIds(newClosingDate);
            if (pendingIds.Count > 0)
            {
                return RedirectToAction(nameof(GeneralJournal), new
                {
                    messageIn = "Pending transactions were found in the requested date range which must be Approved or Denied before a the selected period can be closed can be created.",
                    JIDListIn = pendingIds
                });
            }
            //get retained earning from current income minus exp
            EquityBundle equityBundle = new EquityBundle();
            List<AccountsModel> allAccounts = _dbConnectorService.GetChartOfAccounts();
            bool includeAdjusting = true;

            // calc running net income
            SettingsModel systemSettings = new SettingsModel();
            systemSettings = _dbConnectorService.GetSystemSettings();
            DateTime lastClosingDate = (DateTime)systemSettings.open_close_date;
            string? fromDate = lastClosingDate.ToString("MM-dd-yyyy");
            IncomeStatementBundle incomeBundle = new IncomeStatementBundle();
            string? toDate = openCloseDate;
            foreach (AccountsModel accnt in allAccounts)
            {
                if (accnt.type == "Revenue" && accnt.current_balance != 0)
                {
                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.RevenueAccountsList.Add(accnt);
                    incomeBundle.RevenueAccountsTotal += (double)accnt.current_balance;
                }
                if (accnt.type == "Expense" && accnt.current_balance != 0)
                {
                    accnt.current_balance = _dbConnectorService.GetAccountBalanceForApprovedByDateRange(accnt.number, fromDate, toDate, includeAdjusting);
                    incomeBundle.ExpenseAccountsList.Add(accnt);
                    incomeBundle.ExxpenseAccountsTotal += (double)accnt.current_balance;
                }
            }
            equityBundle.BeginningRetainedEarnings = (double)_dbConnectorService.GetAccountBalanceForApprovedByDateRange(290, fromDate, toDate, includeAdjusting);
            equityBundle.RunningNetIncome = incomeBundle.RevenueAccountsTotal - incomeBundle.ExxpenseAccountsTotal;
            equityBundle.RunningDividends = (double)_dbConnectorService.GetAccountBalanceForApprovedByDateRange(295, fromDate, toDate, includeAdjusting);
            equityBundle.EndRetainedEarnings = equityBundle.BeginningRetainedEarnings + equityBundle.RunningNetIncome - equityBundle.RunningDividends;

            //get revenue and expense transactions in the closing year date range 
            List<TransactionModel> transactions = new List<TransactionModel>();
            transactions = _dbConnectorService.GetCurrentYearClosingTransactions(newClosingDate);

            //resort
            List<TransactionModel> debitTransactions = new List<TransactionModel>();
            List<TransactionModel> creditTransactions = new List<TransactionModel>();

            foreach (var transaction in transactions)
            {
                if (transaction.credit_account > 0)
                {
                    creditTransactions.Add(transaction);
                }
                else if (transaction.debit_account > 0)
                {
                    debitTransactions.Add(transaction);
                }
            }
            var sortedDebitTransactions = debitTransactions
                .OrderBy(t => t.debit_account)
                .ThenBy(t => t.debit_amount)
                .ToList();
            var sortedCreditTransactions = creditTransactions
                .OrderBy(t => t.debit_account)
                .ThenBy(t => t.debit_amount)
                .ToList();


            List<TransactionModel> sortedTransactions = new List<TransactionModel>();
            for (int i = creditTransactions.Count - 1; 0 <= i; i--)
            {
                sortedTransactions.Add(sortedCreditTransactions[i]);
            }

            for (int i = debitTransactions.Count - 1; 0 <= i; i--)
            {
                sortedTransactions.Add(sortedDebitTransactions[i]);
            }

            int nextJournalId = _dbConnectorService.GetNextJournalId();

            //reverse amounts
            foreach (TransactionModel transaction in sortedTransactions)
            {
                if (transaction.credit_amount > 0)
                {
                    TransactionModel nextJournalTransactionItem = new TransactionModel();
                    nextJournalTransactionItem.transaction_number = _dbConnectorService.GetNextTransactionId();
                    nextJournalTransactionItem.debit_account = transaction.credit_account;
                    nextJournalTransactionItem.debit_amount = transaction.credit_amount;
                    nextJournalTransactionItem.credit_account = 0;
                    nextJournalTransactionItem.credit_amount = 0;
                    nextJournalTransactionItem.is_adjusting = "true";
                    nextJournalTransactionItem.status = "Pending";
                    nextJournalTransactionItem.journal_id = nextJournalId;
                    nextJournalTransactionItem.journal_date = DateTime.Now;
                    nextJournalTransactionItem.created_by = userDetails.ScreenName;
                    nextJournalTransactionItem.transaction_date = newClosingDate;
                    nextJournalTransactionItem.is_adjusting = "true";
                    nextJournalTransactionItem.is_opening = false;
                    nextJournalTransactionItem.journal_description = $"Closing entry created by: {nextJournalTransactionItem.created_by} on {nextJournalTransactionItem.transaction_date}.";
                    _dbConnectorService.AddTransaction(nextJournalTransactionItem);
                }
                else if (transaction.debit_amount > 0)
                {
                    TransactionModel nextJournalTransactionItem = new TransactionModel();
                    nextJournalTransactionItem.transaction_number = _dbConnectorService.GetNextTransactionId();
                    nextJournalTransactionItem.credit_account = transaction.debit_account;
                    nextJournalTransactionItem.credit_amount = transaction.debit_amount;
                    nextJournalTransactionItem.debit_account = 0;
                    nextJournalTransactionItem.debit_amount = 0;
                    nextJournalTransactionItem.is_adjusting = "true";
                    nextJournalTransactionItem.status = "Pending";
                    nextJournalTransactionItem.journal_id = nextJournalId;
                    nextJournalTransactionItem.journal_date = DateTime.Now;
                    nextJournalTransactionItem.created_by = userDetails.ScreenName;
                    nextJournalTransactionItem.transaction_date = newClosingDate;
                    nextJournalTransactionItem.is_adjusting = "true";
                    nextJournalTransactionItem.is_opening = false;
                    nextJournalTransactionItem.journal_description = $"Closing entry created by: {nextJournalTransactionItem.created_by} on {nextJournalTransactionItem.transaction_date}.";
                    _dbConnectorService.AddTransaction(nextJournalTransactionItem);
                }
            }

            //add retained earning transaction to adjusting entry
            TransactionModel retainedEarningsJournalTransactionItem = new TransactionModel();
            retainedEarningsJournalTransactionItem.transaction_number = _dbConnectorService.GetNextTransactionId();
            retainedEarningsJournalTransactionItem.credit_account = 290; //retained earnings
            retainedEarningsJournalTransactionItem.credit_amount = equityBundle.RunningNetIncome;
            retainedEarningsJournalTransactionItem.is_adjusting = "true";
            retainedEarningsJournalTransactionItem.status = "Pending";
            retainedEarningsJournalTransactionItem.journal_id = nextJournalId;
            retainedEarningsJournalTransactionItem.journal_date = DateTime.Now;
            retainedEarningsJournalTransactionItem.created_by = userDetails.ScreenName;
            retainedEarningsJournalTransactionItem.transaction_date = newClosingDate;
            retainedEarningsJournalTransactionItem.is_adjusting = "true";
            retainedEarningsJournalTransactionItem.is_opening = false;
            retainedEarningsJournalTransactionItem.journal_description = $"Closing entry created by: {retainedEarningsJournalTransactionItem.created_by} on {retainedEarningsJournalTransactionItem.transaction_date}.";
            _dbConnectorService.AddTransaction(retainedEarningsJournalTransactionItem);
            //update retained earnings account bal

            // set the new closing date
            systemSettings.open_close_date = DateTime.Parse(toDate);
            systemSettings.open_close_on_date = DateTime.Now;
            systemSettings.closing_user = userDetails.ScreenName;
            _dbConnectorService.UpdateSystemSettings(systemSettings);

            return RedirectToAction(nameof(ViewJournalDetails), new
            {
                Id = nextJournalId
            });
        }
    }
}


