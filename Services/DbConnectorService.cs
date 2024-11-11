/*No use or licensing of any kind is authorized with this software. By receiving it, you agree that it will not be used without the express written consent of each of its contributors. This notification supersedes any past agreement, whether written or implied.*/
using oa.Models;
using MySql.Data.MySqlClient;
using oa.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace oa.Services
{
    public class DbConnectorService
    {
        public DbConnectorService() { }
        /*
        * Gets a user's date of birth based on their email
        */
        public async Task<string> GetUserDateOfBirthByEmailAsync(string userEmail)
        {
            string? foundDOB = "";
            try
            {
                using var connection = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                await connection.OpenAsync();
                using var cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "Select DateofBirth from on_account.Users WHERE Email = @userEmail";
                cmd.Parameters.AddWithValue("@userEmail", userEmail);
                await cmd.ExecuteNonQueryAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    foundDOB = reader.GetValue(0).ToString();
                }

                foundDOB = foundDOB.Substring(0, 10);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundDOB;
        }
        /*
         * Gets a users email from thier login username
         */
        public string GetUserEmail(string usernameIn)
        {
            string? foundEmail = "";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT Email FROM on_account.Users WHERE ScreenName = @ScreenName;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@ScreenName", usernameIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    foundEmail = reader1.GetString(0);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundEmail;
        }
        /*
         * Get list of emails pertaining to users of an administrative/managerial role
         */
        public List<String> GetAdministrativeEmails()
        {
            List<String>? foundAdministrativeEmails = new List<string>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT Email FROM on_account.Users WHERE UserRole = \"Administrator\" OR UserRole = \"Manager\";";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    foundAdministrativeEmails.Add(reader1.GetString(0));
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return foundAdministrativeEmails;
        }
        /*
         * Get list of emails pertaining to users of managerial role
         */
        public List<String> GetManagerEmails()
        {
            List<String>? foundManagerEmails = new List<string>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT Email FROM on_account.Users WHERE UserRole = \"Manager\";";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    foundManagerEmails.Add(reader1.GetString(0));
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return foundManagerEmails;
        }
        /*
         * Gets a users details from db by Id
         */
        public AppUserModel GetUserDetailsById(string idIn)
        {
            var foundUser = new AppUserModel();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.Users WHERE Id = @Id;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@Id", idIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    foundUser.Id = reader1.IsDBNull(0) ? null : reader1.GetString(0);
                    foundUser.ScreenName = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                    foundUser.FirstName = reader1.IsDBNull(2) ? null : reader1.GetString(2);
                    foundUser.LastName = reader1.IsDBNull(3) ? null : reader1.GetString(3);
                    foundUser.PhoneNumber = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    foundUser.DateofBirth = reader1.IsDBNull(5) ? null : reader1.GetDateTime(5);
                    foundUser.Address = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    foundUser.City = reader1.IsDBNull(7) ? null : reader1.GetString(7);
                    foundUser.State = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    foundUser.Zip = reader1.IsDBNull(9) ? null : reader1.GetString(9);
                    foundUser.UserRole = reader1.IsDBNull(10) ? null : reader1.GetString(10);
                    foundUser.ActiveStatus = reader1.IsDBNull(11) ? null : reader1.GetBoolean(11).ToString();
                    foundUser.AcctSuspensionDate = reader1.IsDBNull(12) ? null : reader1.GetDateTime(12);
                    foundUser.AcctReinstatementDate = reader1.IsDBNull(13) ? null : reader1.GetDateTime(13);
                    foundUser.LastPasswordChangedDate = reader1.IsDBNull(14) ? null : reader1.GetDateTime(14).ToString();
                    foundUser.PasswordResetDays = reader1.IsDBNull(15) ? null : reader1.GetString(15);
                    foundUser.File = reader1.IsDBNull(16) ? null : reader1.GetString(16);
                    foundUser.UserName = reader1.IsDBNull(17) ? null : reader1.GetString(17);
                    foundUser.NormalizedUserName = reader1.IsDBNull(18) ? null : reader1.GetString(18);
                    foundUser.Email = reader1.IsDBNull(19) ? null : reader1.GetString(19);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundUser;
        }
        /*
         * Updates the database with a users details
         */
        public bool UpdateUserDetails(AppUserModel userIn)
        {
            // sudo code illustrating two methods for capturing data for a change log.
            // So this is one way you can do it.
            DateTime today = DateTime.Now;
            List<LogModel> NewList = new List<LogModel>();
            AppUserModel oldUserInfo = GetUserDetailsById(userIn.Id);
            if (userIn.ScreenName != oldUserInfo.ScreenName)
            {
                LogModel newLog = new LogModel();
                newLog.ChangeDate = today;
                newLog.UserId = userIn.Id;
                newLog.ChangedFrom = oldUserInfo.ScreenName;
                newLog.ChangedTo = userIn.ScreenName;
                newLog.UserId = userIn.Id;
                NewList.Add(newLog);
            }
            //then over and over again or.......

            // This is another way.  See the UserModelComparer class that is being called. Pretty sure this is the better option. The results need to be sanitized a bit more.
            UserModelComparer userModelComparer = new UserModelComparer();
            List<LogModel> anotherLogList = userModelComparer.Compare(oldUserInfo, userIn);
            // becuase anotherLogList is a list you can iterate and send it to a logging method.
            if (anotherLogList.Count > 0)
            {
                //send the list to a method that inserts the logged endtries into the database.
                AddLogs(anotherLogList);
            }

            bool Succeeded = false;
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.Users SET ScreenName = @ScreenName, FirstName = @FirstName, " +
                "LastName = @LastName, PhoneNumber = @PhoneNumber, Address = @Address, City = @City, State = @State, " +
                "Zip = @Zip, DateofBirth = @DateofBirth, UserRole = @UserRole, UserName = @UserName, " +
                "Email = @Email, NormalizedUserName = @NormalizedUserName, AcctSuspensionDate = @AcctSuspensionDate, " +
                "AcctReinstatementDate = @AcctReinstatementDate, LastPasswordChangedDate = @LastPasswordChangedDate, " +
                "PasswordResetDays = @PasswordResetDays WHERE Id LIKE @Id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@Id", userIn.Id);
                cmd1.Parameters.AddWithValue("@ScreenName", userIn.ScreenName);
                cmd1.Parameters.AddWithValue("@FirstName", userIn.FirstName);
                cmd1.Parameters.AddWithValue("@LastName", userIn.LastName);
                cmd1.Parameters.AddWithValue("@PhoneNumber", userIn.PhoneNumber);
                cmd1.Parameters.AddWithValue("@Address", userIn.Address);
                cmd1.Parameters.AddWithValue("@City", userIn.City);
                cmd1.Parameters.AddWithValue("@State", userIn.State);
                cmd1.Parameters.AddWithValue("@Zip", userIn.Zip);
                cmd1.Parameters.AddWithValue("@DateofBirth", userIn.DateofBirth);

                //remove the old role before setting a new one.
                AppUserModel currentUserModel = GetUserDetailsById(userIn.Id);
                if (currentUserModel.UserRole != userIn.UserRole)
                {
                    DeleteUserRole(currentUserModel.Id);
                }
                AssignUserRole(currentUserModel.Id, userIn.UserRole);

                cmd1.Parameters.AddWithValue("@UserRole", userIn.UserRole);
                cmd1.Parameters.AddWithValue("@UserName", userIn.Email);
                cmd1.Parameters.AddWithValue("@Email", userIn.Email);
                cmd1.Parameters.AddWithValue("@NormalizedUserName", userIn.Email.ToUpper());
                cmd1.Parameters.AddWithValue("@AcctSuspensionDate", userIn.AcctSuspensionDate);
                cmd1.Parameters.AddWithValue("@AcctReinstatementDate", userIn.AcctReinstatementDate);
                cmd1.Parameters.AddWithValue("@LastPasswordChangedDate", userIn.LastPasswordChangedDate);
                cmd1.Parameters.AddWithValue("@PasswordResetDays", userIn.PasswordResetDays);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
                Succeeded = true;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Succeeded;
        }
        /*
         * Updates the database to enable scheduled user lockout events
         */
        public void UpdateLockout(AppUser userIn)//admin controller add _dbConnectorService.logModelCreator(****find userid*****,"Lockout disabled for: "+Id, ""); 
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.Users SET LockoutEnd = @LoukoutEnd, AcctSuspensionDate = @AcctSuspensionDate, AccountReinstatementDate = @AccountReinstatementDate WHERE id LIKE @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", userIn.Id);
                cmd1.Parameters.AddWithValue("@AcctSuspensionDate", userIn.AcctSuspensionDate);
                cmd1.Parameters.AddWithValue("@AcctReinstatementDate", userIn.AcctReinstatementDate);
                cmd1.Parameters.AddWithValue("@LockoutEnd", userIn.AcctReinstatementDate);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Updates the database to enable scheduled user lockout events
         */
        public void immediateLockout(string IdIn)//admin controller add _dbConnectorService.logModelCreator(****find userid*****,"Lockout disabled for: "+Id, ""); 
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.Users SET LockoutEnabled = @LockoutEnabled WHERE id LIKE @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", IdIn);
                cmd1.Parameters.AddWithValue("@LockoutEnabled", 1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Updates the database to enable scheduled user lockout events
         */
        public void disableLockout(string IdIn)//admin controller add _dbConnectorService.logModelCreator(****find userid*****,"Lockout disabled for: "+Id, ""); 
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.Users SET LockoutEnabled = @LockoutEnabled WHERE id LIKE @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", IdIn);
                cmd1.Parameters.AddWithValue("@LockoutEnabled", 0);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Gets a list of account type options
         */
        public List<SelectListItem> GetAccountTypeOptions()
        {
            var resultsList = new List<SelectListItem>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account_type_options;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();

                while (reader1.Read())
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = reader1.GetString(0);
                    resultsList.Add(item);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultsList;
        }
        /*
         * Gets a list of normal side options
         */
        public List<SelectListItem> GetNormalSideOptions()
        {
            var resultsList = new List<SelectListItem>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account_normal_side_options;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();

                while (reader1.Read())
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = reader1.GetString(0);
                    resultsList.Add(item);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultsList;
        }
        /*
         * Gets a list of previously used password hashes
         */
        public List<PassHashModel> GetPassHashList()
        {
            var resultsList = new List<PassHashModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.pass_hash;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    PassHashModel userHash = new PassHashModel();
                    userHash.Id = reader1.GetInt16(0);
                    userHash.userId = reader1.GetString(1);
                    userHash.passhash = reader1.GetString(2);
                    resultsList.Add(userHash);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultsList;
        }
        /*
         * Gets a add a password hash to the pass_hash table
         */
        public void StorePassHash(string userId, string passhash)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "INSERT INTO on_account.pass_hash (userId, passhash) VALUES (@userId, @passhash)";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@userId", userId);
                cmd1.Parameters.AddWithValue("@passhash", passhash);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Gets a list of user roles
         */
        public List<RoleModel> GetUserRole(string userRoleIn)
        {
            List<RoleModel> foundRoles = new List<RoleModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT FirstName,LastName,Username FROM on_account.Users Where UserRole=@Role;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@Role", userRoleIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    RoleModel role = new RoleModel();
                    role.firstName = reader1.GetString(0);
                    role.lastName = reader1.GetString(1);
                    role.email = reader1.GetString(2);
                    foundRoles.Add(role);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundRoles;
        }
        /*
         * Deletes one users record from the UserRoles table. This helps to prevent the user from having more than on role asssigned to a single account.
         */
        public void DeleteUserRole(string uidIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "DELETE FROM on_account.UserRoles WHERE UserId = @UserId;";
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                conn1.Open();
                cmd1.Parameters.AddWithValue("@UserId", uidIn);
                cmd1.ExecuteNonQuery();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Gets a role id based on the role type as a string
         */
        public string GetRoleId(string userRoleIn)
        {
            string foundId = "";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.UserRoles;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    if(reader1.GetString(1) == userRoleIn)
                    {
                        foundId = reader1.GetString(0);
                    } 
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundId;
        }
        /*
         * Updates a users role in the UserRoles juntion table
         */
        public void AssignUserRole(string uidIn, string uRoleIn)
        {
            //get role id
            string newRoleId = GetRoleId(uRoleIn);
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.UserRoles SET RoleId = @RoleId WHERE UserId = @UserId;";
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                conn1.Open();
                cmd1.Parameters.AddWithValue("@UserId", uidIn);
                cmd1.Parameters.AddWithValue("@RoleId", newRoleId);
                cmd1.ExecuteNonQuery();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Gets all accounts
         */
        public List<AccountsModel> GetChartOfAccounts()
        {
            List<AccountsModel> accountsModels = new List<AccountsModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    AccountsModel account = new AccountsModel();
                    account.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    account.name = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                    account.number = reader1.IsDBNull(2) ? null : reader1.GetInt32(2);
                    account.sort_priority = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    account.normal_side = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    account.description = reader1.IsDBNull(5) ? null : reader1.GetString(5);
                    account.type = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    account.term = reader1.IsDBNull(7) ? null : reader1.GetString(7);
                    account.statement_type = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    account.account_creation_date = reader1.IsDBNull(9) ? null : reader1.GetDateTime(9);
                    account.opening_transaction_num = reader1.IsDBNull(10) ? null : reader1.GetString(10);
                    account.current_balance = reader1.IsDBNull(11) ? null : reader1.GetDecimal(11);
                    account.created_by = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    account.account_status = reader1.IsDBNull(13) ? null : reader1.GetString(13);
                    account.starting_balance = reader1.IsDBNull(14) ? null : reader1.GetDecimal(14);
                    account.comments = reader1.IsDBNull(15) ? null : reader1.GetString(15);

                    accountsModels.Add(account);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return accountsModels;
        }
        // gets a list of all accounts that have the type inputed
        public List<AccountsModel> GetAccountsOnType(string? type)
        {
            List<AccountsModel> accountsModels = new List<AccountsModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account where type = @AccountType and current_balance <> 0";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@AccountType", type);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    AccountsModel account = new AccountsModel();
                    account.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    account.name = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                    account.number = reader1.IsDBNull(2) ? null : reader1.GetInt32(2);
                    account.sort_priority = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    account.normal_side = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    account.description = reader1.IsDBNull(5) ? null : reader1.GetString(5);
                    account.type = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    account.term = reader1.IsDBNull(7) ? null : reader1.GetString(7);
                    account.statement_type = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    account.account_creation_date = reader1.IsDBNull(9) ? null : reader1.GetDateTime(9);
                    account.opening_transaction_num = reader1.IsDBNull(10) ? null : reader1.GetString(10);
                    account.current_balance = reader1.IsDBNull(11) ? null : reader1.GetDecimal(11);
                    account.created_by = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    account.account_status = reader1.IsDBNull(13) ? null : reader1.GetString(13);
                    account.starting_balance = reader1.IsDBNull(14) ? null : reader1.GetDecimal(14);
                    account.comments = reader1.IsDBNull(15) ? null : reader1.GetString(15);

                    accountsModels.Add(account);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return accountsModels;
        }
        /*
         * Gets all accounts that are non zero balance
         */
        public List<AccountsModel> GetNonZeroAccounts()
        {
            List<AccountsModel> accountsModels = new List<AccountsModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account WHERE current_balance <> 0";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    AccountsModel account = new AccountsModel();
                    account.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    account.name = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                    account.number = reader1.IsDBNull(2) ? null : reader1.GetInt32(2);
                    account.sort_priority = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    account.normal_side = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    account.description = reader1.IsDBNull(5) ? null : reader1.GetString(5);
                    account.type = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    account.term = reader1.IsDBNull(7) ? null : reader1.GetString(7);
                    account.statement_type = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    account.account_creation_date = reader1.IsDBNull(9) ? null : reader1.GetDateTime(9);
                    account.opening_transaction_num = reader1.IsDBNull(10) ? null : reader1.GetString(10);
                    account.current_balance = reader1.IsDBNull(11) ? null : reader1.GetDecimal(11);
                    account.created_by = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    account.account_status = reader1.IsDBNull(13) ? null : reader1.GetString(13);
                    account.starting_balance = reader1.IsDBNull(14) ? null : reader1.GetDecimal(14);
                    account.comments = reader1.IsDBNull(15) ? null : reader1.GetString(15);

                    accountsModels.Add(account);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return accountsModels;
        }
        /*
         * Gets a single account based on its id
         */
        public AccountsModel GetAccount(string idIn)
        {
            AccountsModel account = new AccountsModel();
            try
            {                
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account WHERE number = @Id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@Id", idIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {                    
                    account.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    account.name = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                    account.number = reader1.IsDBNull(2) ? null : reader1.GetInt32(2);
                    account.sort_priority = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    account.normal_side = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    account.description = reader1.IsDBNull(5) ? null : reader1.GetString(5);
                    account.type = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    account.term = reader1.IsDBNull(7) ? null : reader1.GetString(7);
                    account.statement_type = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    account.account_creation_date = reader1.IsDBNull(9) ? null : reader1.GetDateTime(9);
                    account.opening_transaction_num = reader1.IsDBNull(10) ? null : reader1.GetString(10);
                    account.current_balance = reader1.IsDBNull(11) ? null : reader1.GetDecimal(11);
                    account.created_by = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    account.account_status = reader1.IsDBNull(13) ? null : reader1.GetString(13);
                    account.starting_balance = reader1.IsDBNull(14) ? null : reader1.GetDecimal(14);
                    account.comments = reader1.IsDBNull(15) ? null : reader1.GetString(15);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return account;
        }
        /*
         * Gets a sindgle normal side based on its account number
         */
        public string? GetAccountNormalSideByNumber(string accountNumIn)
        {
            string? normal_side = "";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.account WHERE number = @number";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@number", accountNumIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    normal_side = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return normal_side;
        }
        /*
         * Creates a new account in the db and return the updated model with a new accountId number
         * Note that this is only sutable for new accounts
         */
        public AccountsModel CreateNewAccount(AccountsModel accountModelIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "INSERT INTO on_account.account (name, number, sort_priority, normal_side, description, type, term, statement_type, opening_transaction_num, current_balance, created_by, account_status, starting_balance, account_creation_date, comments)" +
                    " VALUES (@name, @number, @sort_priority, @normal_side, @description, @type, @term, @statement_type, @opening_transaction_num, @current_balance, @created_by, @account_status, @starting_balance, @account_creation_date, @comments)";
                
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@name", accountModelIn.name);
                cmd1.Parameters.AddWithValue("@number", accountModelIn.number);
                cmd1.Parameters.AddWithValue("@sort_priority", accountModelIn.sort_priority);
                cmd1.Parameters.AddWithValue("@normal_side", accountModelIn.normal_side);
                cmd1.Parameters.AddWithValue("@description", accountModelIn.description);
                cmd1.Parameters.AddWithValue("@type", accountModelIn.type);
                cmd1.Parameters.AddWithValue("@term", accountModelIn.term);
                cmd1.Parameters.AddWithValue("@statement_type", accountModelIn.statement_type);
                cmd1.Parameters.AddWithValue("@opening_transaction_num", accountModelIn.opening_transaction_num);
                cmd1.Parameters.AddWithValue("@current_balance", accountModelIn.current_balance);
                cmd1.Parameters.AddWithValue("@created_by", accountModelIn.created_by);
                cmd1.Parameters.AddWithValue("@account_status", accountModelIn.account_status);
                cmd1.Parameters.AddWithValue("@starting_balance", accountModelIn.starting_balance);
                cmd1.Parameters.AddWithValue("@account_creation_date", accountModelIn.account_creation_date);
                cmd1.Parameters.AddWithValue("@comments", accountModelIn.comments);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            logModelCreator(accountModelIn.created_by, "Added new Account: "+accountModelIn.name, "new ID: "+accountModelIn.number);
            AccountsModel modelWithId = GetAccount(accountModelIn.name);
            return modelWithId;
        }
        /*
         * Updates account in the db and return the updated model with a new accountId number
         * 
         */
        public void UpdateExistingAccount(AccountsModelEdit accountModelIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.account SET name = @name, number = @number, sort_priority = @sort_priority, normal_side = @normal_side, description = @description, type = @type, term = @term, statement_type = @statement_type, opening_transaction_num = @opening_transaction_num, current_balance = @current_balance, created_by = @created_by, account_status = @account_status, starting_balance = @starting_balance, account_creation_date = @account_creation_date, comments = @comments WHERE id = @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", accountModelIn.id);
                cmd1.Parameters.AddWithValue("@name", accountModelIn.name);
                cmd1.Parameters.AddWithValue("@number", accountModelIn.number);
                cmd1.Parameters.AddWithValue("@sort_priority", accountModelIn.sort_priority);
                cmd1.Parameters.AddWithValue("@normal_side", accountModelIn.normal_side);
                cmd1.Parameters.AddWithValue("@description", accountModelIn.description);
                cmd1.Parameters.AddWithValue("@type", accountModelIn.type);
                if (accountModelIn.term == null) { accountModelIn.term = ""; } else { cmd1.Parameters.AddWithValue("@term", accountModelIn.term); }
                cmd1.Parameters.AddWithValue("@statement_type", accountModelIn.statement_type);
                cmd1.Parameters.AddWithValue("@opening_transaction_num", accountModelIn.opening_transaction_num);
                cmd1.Parameters.AddWithValue("@current_balance", accountModelIn.current_balance);
                cmd1.Parameters.AddWithValue("@created_by", accountModelIn.created_by);
                cmd1.Parameters.AddWithValue("@account_status", accountModelIn.account_status);
                cmd1.Parameters.AddWithValue("@starting_balance", accountModelIn.starting_balance);
                cmd1.Parameters.AddWithValue("@account_creation_date", accountModelIn.account_creation_date);
                cmd1.Parameters.AddWithValue("@comments", accountModelIn.comments);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            logModelCreator(accountModelIn.created_by, "Account: "+ accountModelIn.name+" updated", "");
        }
        /*
         * Updates account balance
         * 
         */
        public void UpdateAccountBalance(string acctNum, double newBal)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.account SET current_balance = @current_balance WHERE number = @number";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);

                cmd1.Parameters.AddWithValue("@number", acctNum);
                cmd1.Parameters.AddWithValue("@current_balance", newBal);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public double CalculateAccountBalance(string? accountNumberIn)
        {
            //get normal side
            string? normal_side = GetAccountNormalSideByNumber(accountNumberIn);

            double dr = 0;
            double cr = 0;
            double balance = 0;
            List<TransactionModel> accountTransactions = GetApprovedAccountTransactions(accountNumberIn);

            for (int i = 0; i < accountTransactions.Count; i++)
            {
                if (accountTransactions[i].credit_amount != null)
                {
                    cr += (double)accountTransactions[i].credit_amount;
                }
                if (accountTransactions[i].debit_amount != null)
                {
                    dr += (double)accountTransactions[i].debit_amount;
                }
            }
            // calc balance based on normal side.
            if(normal_side == "Debit")
            {
                balance = dr - cr;
            } else if (normal_side == "Credit")
            {
                balance = cr - dr;
            }
            UpdateAccountBalance(accountNumberIn,balance);
            return balance;
        }
        /*
         * Adds a single transaction 
         */
        public void AddTransaction(TransactionModel transactionIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "INSERT INTO on_account.transaction (debit_account, debit_amount, credit_account, credit_amount, transaction_date, created_by, status, is_opening, description, journal_id, transaction_number, journal_description, journal_date, supporting_document) " + 
                    "VALUES (@debit_account, @debit_amount, @credit_account, @credit_amount, @transaction_date, @created_by, @status, @is_opening, @description, @journal_id, @transaction_number, @journal_description, @journal_date, @supporting_document)";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@debit_account", transactionIn.debit_account);
                cmd1.Parameters.AddWithValue("@debit_amount", transactionIn.debit_amount);
                cmd1.Parameters.AddWithValue("@credit_account", transactionIn.credit_account);
                cmd1.Parameters.AddWithValue("@credit_amount", transactionIn.credit_amount);
                cmd1.Parameters.AddWithValue("@transaction_date", transactionIn.transaction_date?.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
                cmd1.Parameters.AddWithValue("@created_by", transactionIn.created_by);
                cmd1.Parameters.AddWithValue("@status", transactionIn.status);
                cmd1.Parameters.AddWithValue("@is_opening", transactionIn.is_opening);
                cmd1.Parameters.AddWithValue("@description", transactionIn.description);
                cmd1.Parameters.AddWithValue("@journal_id", transactionIn.journal_id);
                cmd1.Parameters.AddWithValue("@transaction_number", transactionIn.transaction_number);
                cmd1.Parameters.AddWithValue("@journal_description", transactionIn.journal_description);
                cmd1.Parameters.AddWithValue("@journal_date", transactionIn.journal_date);
                cmd1.Parameters.AddWithValue("@supporting_document", transactionIn.supporting_document);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            string logString = "Transaction added to Account: ";
            string accountName = "";
            string? accountNumber = "";
            AccountsModel currentAccount = new AccountsModel();
            if (transactionIn.debit_account != 0)
            {
                accountNumber =transactionIn.debit_account.ToString();
                currentAccount = GetAccount(accountNumber);
            }
            else if (transactionIn.credit_account != 0)
            {
                accountNumber = transactionIn.credit_account.ToString();
                currentAccount = GetAccount(accountNumber);
            }
            logString = logString + currentAccount.name;
            logModelCreator(transactionIn.created_by,logString,"");
            accountNumber = "";
            if (transactionIn.credit_account != 0)
            {
                accountNumber = transactionIn.credit_account.ToString();
            }
            else if (transactionIn.debit_amount != 0)
            {
                accountNumber = transactionIn.debit_account.ToString();
            }
            double newBal = CalculateAccountBalance(accountNumber);
            UpdateAccountBalance(accountNumber, newBal);
        }

        /*
         * Adds a single transaction
         */
        public void AddSupportingDocs(int? idIn, string? filenameIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "INSERT INTO on_account.supporting_docs (journal_id, file_name) VALUES (@journal_id, @file_name)";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@journal_id", idIn);
                cmd1.Parameters.AddWithValue("@file_name", filenameIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /*
        * Gets the list of documents associated with a journal entry
        */
        public List<string> GetSupportingDocuments(int id)
        {
            List<string> foundNames = new List<string>();
            try
            {
                var connection = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                connection.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "Select * from on_account.supporting_docs WHERE journal_id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string foundName = reader.GetString(2);
                    foundNames.Add(foundName);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundNames;
        }

        /*
         * creates the log model to add to the database
         * 
         * when calling if not changing field but adding field before is used for text of what happened ex creating a user is "added user" to the fieldbefore and field after is an empty string
         */
        public void logModelCreator(string userID, string fieldBefore, string fieldAfter)
        {
            
            LogModel newLog=new LogModel();
            newLog.UserId = userID;
            newLog.ChangeDate = DateTime.Now;
            newLog.ChangedFrom = fieldBefore;
            newLog.ChangedTo = fieldAfter;
            AddLog(newLog);
        }


        /*
         * Gets a list of the logs account based on its name
         */
        public List<LogModel> GetLogs()
        {
            List<LogModel> logs = new List<LogModel>();
            
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.log";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    LogModel log = new LogModel();
                    log.Id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    log.ChangeDate = reader1.IsDBNull(1) ? null : reader1.GetDateTime(1);
                    log.UserId = reader1.IsDBNull(2) ? null : reader1.GetString(2);
                    log.ChangedFrom = reader1.IsDBNull(3) ? null : reader1.GetString(3);
                    log.ChangedTo = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    logs.Add(log);

                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return logs;
        }

        public List<LogModel> GetAccountLogs(string? id)
        {
            List<LogModel> logs = new List<LogModel>();
            id = "%" + id + "%";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.log where ChangedFrom like @AccountID";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@AccountID", id);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    LogModel log = new LogModel();
                    log.Id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    log.ChangeDate = reader1.IsDBNull(1) ? null : reader1.GetDateTime(1);
                    log.UserId = reader1.IsDBNull(2) ? null : reader1.GetString(2);
                    log.ChangedFrom = reader1.IsDBNull(3) ? null : reader1.GetString(3);
                    log.ChangedTo = reader1.IsDBNull(4) ? null : reader1.GetString(4);
                    logs.Add(log);

                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return logs;
        }
        /*
         * Adds a single log to the db
         */
        public void AddLog(LogModel logIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "INSERT on_account.log (ChangeDate, UserId, ChangedFrom, ChangedTo)" +
                    " VALUES (@ChangeDate, @UserId, @ChangedFrom, @ChangedTo)";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@ChangeDate", logIn.ChangeDate);
                cmd1.Parameters.AddWithValue("@UserId", logIn.UserId);
                cmd1.Parameters.AddWithValue("@ChangedFrom", logIn.ChangedFrom);
                cmd1.Parameters.AddWithValue("@ChangedTo", logIn.ChangedTo);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Adds a list logs to the db
         */
        public void AddLogs(List<LogModel> logIn)
        {
            try
            {
                for (int i = 0; i < logIn.Count; i++) {
                    using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                    string command = "INSERT on_account.log (ChangeDate, UserId, ChangedFrom, ChangedTo)" +
                    " VALUES (@ChangeDate, @UserId, @ChangedFrom, @ChangedTo)";
                    conn1.Open();
                    MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                    cmd1.Parameters.AddWithValue("@ChangeDate", logIn[i].ChangeDate);
                    cmd1.Parameters.AddWithValue("@UserId", logIn[i].UserId);
                    cmd1.Parameters.AddWithValue("@ChangedFrom", logIn[i].ChangedFrom);
                    cmd1.Parameters.AddWithValue("@ChangedTo", logIn[i].ChangedTo);
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                    reader1.Close();
                    conn1.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /*
         * Gets all transactions
         */
        public List<TransactionModel> GetAllTransactions()
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
         * Gets all transactions
         */
        public List<TransactionModel> GetAllTransactionsByJournalRange(int? lowerBoundIn,int? upperBoundIn)
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE journal_id BETWEEN @loawerBoundIn AND @upperBoundIn";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@loawerBoundIn", lowerBoundIn);
                cmd1.Parameters.AddWithValue("@upperBoundIn", upperBoundIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
         * Gets and calculates the totals for all of the journal entries and returns them in a list of journal objects.  This necessary for searching by journal amount in the General Journal
         */
        public List<JournalBundle> GetJournalTotals()
        {
            List<JournalBundle> returnBundle = new List<JournalBundle>();
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Dictionary<int?, double?> journalTotals = new Dictionary<int?, double?>();
            foreach (var transaction in transactionsList)
            {
                if (transaction.journal_id != null && transaction.credit_amount > 0)
                {
                    if (journalTotals.ContainsKey(transaction.journal_id))
                    {
                        journalTotals[transaction.journal_id] += transaction.credit_amount;
                    }
                    else
                    {
                        journalTotals[transaction.journal_id] = transaction.credit_amount;
                    }
                }
            }

            foreach (var kvp in journalTotals)
            {
                JournalBundle bundle = new JournalBundle
                {
                    journal_id = kvp.Key,
                    total_adjustment = kvp.Value
                };
                returnBundle.Add(bundle);
            }

            return returnBundle;
        }

        //only gets transactions that have been approved
        public List<TransactionModel> GetAllApprovedTransactions()
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction where status = approved";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
         * Gets all transaction for a specific account number
         */
        public List<TransactionModel> GetAccountTransactions(string? accountNumberIn)
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                int acctNum = int.Parse(accountNumberIn);
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE debit_account=@number OR credit_account=@number";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@number", acctNum);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    //only return transaction data related to the requested account
                    if(reader1.GetInt32(1) == acctNum)
                    {
                        nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                        nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    }
                    if (reader1.GetInt32(3) == acctNum)
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        //pulls only account transactions based on account number of accounts that are of approved status
        public List<TransactionModel> GetApprovedAccountTransactions(string? accountNumberIn)
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                int acctNum = int.Parse(accountNumberIn);
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE (debit_account=@number OR credit_account=@number) AND status='Approved'";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@number", acctNum);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    //only return transaction data related to the requested account
                    if (reader1.GetInt32(1) == acctNum)
                    {
                        nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                        nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    }
                    if (reader1.GetInt32(3) == acctNum)
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
         * Gets all transaction for a specific account number
         */
        public List<TransactionModel> GetAccountTransactionsByAccountNumber(string? id)
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                int Id = int.Parse(id);
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE debit_account=@id OR credit_account=@id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    //only return transaction data related to the requested account
                    if (reader1.GetInt32(1) == Id)
                    {
                        nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                        nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                        nextTransaction.dr_description = nextTransaction.debit_account.ToString() + " - " + GetAccoutName(nextTransaction.debit_account.ToString());
                    }
                    if (reader1.GetInt32(3) == Id)
                    {
                        nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                        nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
                        nextTransaction.cr_description = nextTransaction.credit_account.ToString() + " - " + GetAccoutName(nextTransaction.credit_account.ToString());
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
         * Gets all transaction for a specific journal number
         */
        public List<TransactionModel> GetAccountTransactionsByJournalNumber(string? jid)
        {
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                int Id = int.Parse(jid);
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE journal_id=@journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@journal_id", Id);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? 0 : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? 0 : reader1.GetDouble(2);                       
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? 0 : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? 0 : reader1.GetInt32(4);
                    nextTransaction.transaction_date = reader1.IsDBNull(5) ?    null : reader1.GetDateTime(5);
                    nextTransaction.created_by = reader1.IsDBNull(6) ? null : reader1.GetString(6);
                    nextTransaction.is_opening = reader1.IsDBNull(7) ? null : reader1.GetBoolean(7);
                    nextTransaction.status = reader1.IsDBNull(8) ? null : reader1.GetString(8);
                    nextTransaction.description = reader1.IsDBNull(9) ? null : reader1.GetString(9);
                    nextTransaction.journal_id = reader1.IsDBNull(10) ? null : reader1.GetInt32(10);
                    nextTransaction.transaction_number = reader1.IsDBNull(11) ? null : reader1.GetInt32(11);
                    nextTransaction.journal_description = reader1.IsDBNull(12) ? null : reader1.GetString(12);
                    nextTransaction.journal_date = reader1.IsDBNull(13) ? null : reader1.GetDateTime(13);
                    nextTransaction.supporting_document = reader1.IsDBNull(14) ? null : reader1.GetString(14);
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionsList;
        }
        /*
        * Gets an accounts name from the account number as a string
        */
        public string GetAccoutName(string number)
        {
            string? foundName = "";
            try
            {
                var connection = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                connection.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT name from on_account.account WHERE number = @number";
                cmd.Parameters.AddWithValue("@number", number);
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    foundName = reader.GetString(0);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundName;
        }
        /*
        * Gets an accounts number from the id as an int and returns the number as a string
        */
        public string GetAccoutNumber(int id)
        {
            string? foundNum = "";
            try
            {
                var connection = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                connection.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "Select number from on_account.account WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    foundNum = reader.GetInt32(0).ToString();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return foundNum;
        }

        /*
         * Gets the next journal id
         */
        public int GetNextJournalId()
        {
            int nextJournalId = 0;
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT MAX(journal_id) FROM on_account.transaction";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    int highestId = reader1.GetInt32(0);
                    nextJournalId = highestId + 1;
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return nextJournalId;
        }
        /*
         * Gets transaction number from journal id
         * 
         */
        public string GetTransactionNum(string? journalId)
        {
            string transactionId = "";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT id FROM on_account.transaction WHERE journal_id = @journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@journal_id", journalId);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                transactionId = reader1.GetInt32(0).ToString();
/*              //currently assigns them all to the first transaction id
 *              while (reader1.Read())
                {
                    transactionId = reader1.GetInt32(0).ToString();
                }*/
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return transactionId;
        }
        /*
         * Gets current journal description
         * 
         */
        public string GetJournalNotes(string? journalNumIn)
        {
            string currentJournalDescription = "";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT journal_description FROM on_account.transaction WHERE journal_id = @journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@journal_id", Int32.Parse(journalNumIn));
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    currentJournalDescription = reader1.IsDBNull(0) ? null : reader1.GetString(0);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return currentJournalDescription;
        }

        /*
         * Updates journal transaction description
         * 
         */
        public void UpdateJournalNotes(string? journalNumIn, string? appendStrIn)
        {

            string currentNotes = GetJournalNotes(journalNumIn);
            string nextNotes = currentNotes + " (" + appendStrIn + ") ";
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.transaction SET journal_description = @journal_description WHERE journal_id = @journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@journal_description", nextNotes);
                cmd1.Parameters.AddWithValue("@journal_id", Int32.Parse(journalNumIn));
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            UpdateTransactionStatus(journalNumIn, "Denied");
        }

        /*
         * Updates journal transaction status
         * 
         */
        public void UpdateTransactionStatus(string? journalNumIn, string? newStatusIn)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.transaction SET status = @status WHERE journal_id = @journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@status", newStatusIn);
                cmd1.Parameters.AddWithValue("@journal_id", Int32.Parse(journalNumIn));
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (newStatusIn == "Approved")
            {
                string transactionId = GetTransactionNum(journalNumIn);
                // add transaction id to journal entry post ref field where journalNumIn == JournalNum
                UpdatePostRef(journalNumIn, transactionId);
            }
        }

        /*
         * Updates the post ref for a journal after it has been approved
         */
        public void UpdatePostRef(string? journalIdIn, string? newRefId)
        {
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "UPDATE on_account.transaction SET transaction_number = @transaction_number WHERE journal_id = @journal_id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);

                cmd1.Parameters.AddWithValue("@transaction_number", newRefId);
                cmd1.Parameters.AddWithValue("@journal_id", journalIdIn);

                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int GetNumJournalsStatus(string? statusIn)
        {
            using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
            string command = "SELECT COUNT(DISTINCT journal_id) FROM on_account.transaction WHERE status=@status";
            conn1.Open();
            MySqlCommand cmd1 = new MySqlCommand(command, conn1);
            cmd1.Parameters.AddWithValue("@status", statusIn);
            int numberOfPendingJournals = Convert.ToInt32(cmd1.ExecuteScalar());
            return numberOfPendingJournals;
        }
        public List<TransactionModel> GetAllTransactionsByJournalStatus(int startingRecordNumber,int endingRecordNumber, string statusIn){
            List<TransactionModel> transactionsList = new List<TransactionModel>();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.transaction WHERE status = @status";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@status", statusIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TransactionModel nextTransaction = new TransactionModel();
                    nextTransaction.id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    nextTransaction.debit_account = reader1.IsDBNull(1) ? null : reader1.GetInt32(1);
                    nextTransaction.debit_amount = reader1.IsDBNull(2) ? null : reader1.GetDouble(2);
                    nextTransaction.credit_account = reader1.IsDBNull(3) ? null : reader1.GetInt32(3);
                    nextTransaction.credit_amount = reader1.IsDBNull(4) ? null : reader1.GetInt32(4);
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
                    transactionsList.Add(nextTransaction);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //get list of journal id's to return
            HashSet<int> uniqueJournalIds = new HashSet<int>();
            foreach (var transaction in transactionsList)
            {
                uniqueJournalIds.Add((int)transaction.journal_id);
            }
            List<int> uniqueJournalIdList = new List<int>(uniqueJournalIds);
            uniqueJournalIdList.Sort();
            List<int> returnIds = new List<int>();
            // adjust the ending record number if on the last page.
            if(endingRecordNumber > uniqueJournalIdList.Count() - 1)
            {
                endingRecordNumber = uniqueJournalIdList.Count() - 1;
            }
            
            for(int i = startingRecordNumber; i <= endingRecordNumber - 1; i++)
            {
                returnIds.Add(uniqueJournalIdList[i]);
            }
            // generate list of transactions to return.
            List<TransactionModel> returnTransactionsList = new List<TransactionModel>();
            foreach(var transaction in transactionsList)
            {
                if (returnIds.Contains((int)transaction.journal_id))
                {
                    returnTransactionsList.Add(transaction);
                }
            }
            return returnTransactionsList;

        }

        public SettingsModel GetSystemSettings()
        {

            SettingsModel settings = new SettingsModel();
            try
            {
                using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                string command = "SELECT * FROM on_account.system_settings";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    
                    settings.Id = reader1.IsDBNull(0) ? null : reader1.GetInt32(0);
                    settings.business_name = reader1.IsDBNull(1) ? null : reader1.GetString(1);
                }
                reader1.Close();
                conn1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return settings;
        }

        public SettingsModel UpdateSystemSettings(SettingsModel settingsIn)
        {
            SettingsModel settings = new SettingsModel();
            if (settingsIn.Id == null) {
                try
                {
                    using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                    string command = "INSERT INTO on_account.system_settings (business_name) VALUES (@business_name)";
                    conn1.Open();
                    MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                    cmd1.Parameters.AddWithValue("@Id", settingsIn.Id);
                    cmd1.Parameters.AddWithValue("@business_name", settingsIn.business_name);
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                    reader1.Close();
                    conn1.Close();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            } 
            else {
                try {
                    using var conn1 = new MySqlConnection(Environment.GetEnvironmentVariable("DbConnectionString"));
                    string command = "UPDATE on_account.system_settings SET business_name = @business_name WHERE Id=@Id";
                    conn1.Open();
                    MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                    cmd1.Parameters.AddWithValue("@Id", settingsIn.Id);
                    cmd1.Parameters.AddWithValue("@business_name", settingsIn.business_name);
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                    reader1.Close();
                    conn1.Close();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            settings = GetSystemSettings();
            return settings;
        }
    }
}




