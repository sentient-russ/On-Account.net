/*No use or licensing of any kind is authorized with this software. By receiving it, you agree that it will not be used without the express written consent of each of its contributors. This notification supersedes any past agreement, whether written or implied.*/
using oa.Models;
using MySql.Data.MySqlClient;
using System;
using oa.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace oa.Services
{
    public class ChartingDataService
    {
        public DbConnectorService _connectorService;

        public ChartingDataService() {

            _connectorService = new DbConnectorService();

        }

        public void GetGrossProfitMarginData(string? fromDateIn = "", string? toDateIn = "", bool includeAdjusting = false)
        {

            using var connection = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
            
            DateTime? fromDate = null;
            DateTime? toDate = DateTime.Now;
            SettingsModel settings = _connectorService.GetSystemSettings();
            if(fromDateIn != "")
            {
                fromDate = DateTime.Parse(fromDateIn);
            } else
            {
                fromDate = settings.open_close_date;
            }
            if(toDateIn != "")
            {
                toDate = DateTime.Parse(toDateIn);
            }

            //public decimal GetAccountBalanceForApprovedByDateRange(int? accountNumberIn, string? dateIn, bool includeAdjusting = false)

            //Get accounts by type
            List<AccountsModel> incomeAccounts = new List<AccountsModel>();
            List<AccountsModel> expenseAccounts = new List<AccountsModel>();


        }

        // Used for: Current Ratio,
        public decimal GetAccountTypeTotalBalance(string? accountType = "")
        {
            List<AccountsModel> assetAccounts = _connectorService.GetAccountsOnType(accountType);

            decimal? assetTotalBalance = 0;
            foreach (AccountsModel account in assetAccounts)
            {
                Console.WriteLine(accountType + "Test: cur_bal: " + account.current_balance); // Debugging
                assetTotalBalance += account.current_balance;
                Console.WriteLine(accountType + "Test: total: " + assetTotalBalance); // Debugging
            }

            return (decimal)assetTotalBalance;
        }

        public decimal GetAccountTypeTotalBalance(string? accountType = "", string? term = "")
        {
            List<AccountsModel> assetAccounts = _connectorService.GetAccountsOnTypeAndTerm(accountType, term);

            decimal? assetTotalBalance = 0;
            foreach (AccountsModel account in assetAccounts)
            {
                Console.WriteLine(accountType + term + "Test: cur_bal: " + account.current_balance); // Debugging
                assetTotalBalance += account.current_balance;
                Console.WriteLine(accountType + "Test: total: " + assetTotalBalance); // Debugging
            }

            return (decimal)assetTotalBalance;
        }

        /*
         * Caculate an accounts balance from the begining of the accounting calandar year to a secified date. 
         *//*
        public decimal GetAccountBalanceForApprovedByDateRange(string? fromDateIn, bool includeAdjusting = false)
        {


            List<TransactionModel> accountTransactions = new List<TransactionModel>();
            try
            {
                string command = "";
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                if (!includeAdjusting)
                {
                    command = "SELECT * FROM on_account.transaction WHERE (debit_account=@accountNumberIn OR credit_account=@accountNumberIn) AND transaction_date BETWEEN @fromDate AND @toDateIn AND status = 'Approved' AND is_adjusting='false'";
                }
                else
                {
                    command = "SELECT * FROM on_account.transaction WHERE (debit_account=@accountNumberIn OR credit_account=@accountNumberIn) AND transaction_date BETWEEN @fromDate AND @toDateIn AND status = 'Approved'";
                }
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@accountNumberIn", accountNumberIn);
                cmd1.Parameters.AddWithValue("@fromDate", fromDate);
                cmd1.Parameters.AddWithValue("@toDateIn", toDate);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    //only return transaction data related to the requested account
                    if (reader1.GetInt32(1) == accountNumberIn)
                    {
                        nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                        nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    }
                    if (reader1.GetInt32(3) == accountNumberIn)
                    {
                        nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                        nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
                    }
                    nextTransaction.transaction_date = reader1.IsDBNull(5) ? null : reader1.GetDateTime(5);
                    nextTransaction.created_by = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    nextTransaction.is_opening = reader1.IsDBNull(7) ? null : reader1.GetBoolean(7);
                    nextTransaction.status = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    nextTransaction.description = reader1.IsDBNull(9) ? null : reader1.GetString(9);
                    nextTransaction.journal_id = reader1.IsDBNull(10) ? null : reader1.GetInt32(10);
                    nextTransaction.transaction_number = reader1.IsDBNull(11) ? null : reader1.GetInt32(11);
                    nextTransaction.journal_description = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    nextTransaction.journal_date = reader1.IsDBNull(13) ? null : reader1.GetDateTime(13);
                    nextTransaction.supporting_document = reader1.IsDBNull(14) ? null : reader1.GetString(14);
                    accountTransactions.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //get normal side
            string? normal_side = GetAccountNormalSideByNumber(accountNumberIn.ToString());
            //calc bal
            decimal dr = 0;
            decimal cr = 0;
            decimal balance = 0;
            for (int i = 0; i < accountTransactions.Count; i++)
            {
                if (accountTransactions[i].credit_amount != null)
                {
                    cr += (decimal)accountTransactions[i].credit_amount;
                }
                if (accountTransactions[i].debit_amount != null)
                {
                    dr += (decimal)accountTransactions[i].debit_amount;
                }
            }
            // calc balance based on normal side.
            if (normal_side == "Debit")
            {
                balance = dr - cr;
            }
            else if (normal_side == "Credit")
            {
                balance = cr - dr;
            }

            return balance;
        }*/

    }
}
