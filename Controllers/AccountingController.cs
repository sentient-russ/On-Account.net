using Microsoft.AspNetCore.Mvc;
using oa.Services;
using oa.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.UI.Services;
using oa.Areas.Identity.Data;

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

        public AccountingController(DbConnectorService connectorService,
            UserService userService, IEmailSender emailSender)
        {
            _dbConnectorService = connectorService;
            _userService = userService;
            currentAccounts = _dbConnectorService.GetChartOfAccounts().OrderBy(a => a.number).ToList();
            accountModel = new AccountsModel();
            this._emailSender = emailSender;
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
                uploadFileName = newFileName;
            }

            for (int j = 0; j < journalEntry.Transactions.Count; j++)
            {
                for (int i = 0; i < journalEntry.Transactions[j].LineItems.Count; i++)
                {
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
                        supporting_document = uploadFileName
                    };

                    _dbConnectorService.AddTransaction(transactionIn);
                }
            }

            return Ok(new { message = "Journal data received successfully", journalData = journalData });
        }
    


    //All users can view accounts details pages
    [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewAccountDetails(string? id)
        {
            List<TransactionModel> currentTransactions = _dbConnectorService.GetAccountTransactions(id);

            for(var i = currentTransactions.Count -1; 0 < i; i--)
            {
                if (currentTransactions[i].status == "Pending")
                {
                    currentTransactions.RemoveAt(i);
                } else if (currentTransactions[i].status == "Denied")
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
        public async Task<IActionResult> GeneralJournal()
        {
            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            List<TransactionModel> currentTransactions = _dbConnectorService.GetAllTransactions();
            for(var i = 0; i < currentTransactions.Count; i++)
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
            return View(currentTransactions);
        }

        //All users can view journal details
        [Authorize(Roles = "Administrator, Manager, Accountant")]
        public async Task<IActionResult> ViewJournalDetails(string? id)
        {
            List<AccountsModel> currentAccounts = _dbConnectorService.GetChartOfAccounts();
            JournalBundle infoBundle = new JournalBundle();
            infoBundle.transactions_list = _dbConnectorService.GetAccountTransactionsByJournalNumber(id);
            infoBundle.journal_id = Int32.Parse(id);
            infoBundle.created_by = infoBundle.transactions_list[0].created_by;
            infoBundle.status = infoBundle.transactions_list[0].status;
            infoBundle.journal_date = infoBundle.transactions_list[0].journal_date;

            for (int i = 0; i < infoBundle.transactions_list.Count; i++)
            {
                infoBundle.transactions_list[i].cr_description = currentAccounts
                    .Where(account => account.number == infoBundle.transactions_list[i].credit_account)
                    .Select(account => account.name)
                    .FirstOrDefault();
                if (infoBundle.transactions_list[i].credit_account != 0)
                {
                    infoBundle.transactions_list[i].cr_description = infoBundle.transactions_list[i].credit_account + " - " + infoBundle.transactions_list[i].cr_description;
                } else
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
            infoBundle.total_adjustment = balance.ToString();
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
        //Only the manager can approve or deny a transaction.
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveJournal(string? id)
        {
            _dbConnectorService.UpdateTransactionStatus(id, "Approved");
           // need log update here
            return RedirectToAction(nameof(GeneralJournal));
        }

        // Viewable and usable by ANY logged-in user/role
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        public async Task<IActionResult> EmailAdmin(string? id)
        {
            List<String> administrativeEmails = _dbConnectorService.GetAdministrativeEmails();

            AppUser emailBundle = new AppUser();

            emailBundle.AdminManagerEmails = administrativeEmails;

            return View(emailBundle);
        }
        [Authorize(Roles = "Manager, Accountant, Administrator")]
        [HttpPost]
        public async Task<IActionResult> SendEmailAdmin([Bind("Id, Email, Subject, Message")] AppUserModel detailsIn)
        {
            AppUserModel appUserMessage = new AppUserModel();

            appUserMessage = detailsIn;

            // Task runs in a standalone discard while the redirection takes place. (No more await)
            _ = Task.Run(() => _emailSender.SendEmailAsync(appUserMessage.Email, appUserMessage.Subject, appUserMessage.Message));

            return RedirectToAction("Index", "Home");
        }
    }
}