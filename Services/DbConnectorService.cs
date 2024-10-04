using oa.Models;
using MySql.Data.MySqlClient;
using oa.Areas.Identity.Data;
using oa.Migrations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Collections;

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
                    foundDOB = reader.GetString(0);
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
                if (currentUserModel.UserRole == null || currentUserModel.UserRole == "")
                {
                    //do nothing
                }
                else
                {
                    //DeleteUserRole(currentUserModel.Id);
                    AssignUserRole(currentUserModel.Id, userIn.UserRole);
                    //get roles list with id's
                    //update the roles table with user id and role.
                }

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
        public void UpdateLockout(AppUser userIn)
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
        public void immediateLockout(string IdIn)
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
        public void disableLockout(string IdIn)
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
                string command = "UPDATE on_account.UserRoles SET RoleId = @RoleId WHEREUserId = @UserId;";
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

    }

}




