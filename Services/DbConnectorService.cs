using OnAccount.Models;
using MySql.Data.MySqlClient;
using OnAccount.Areas.Identity.Data;
using OnAccount.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace OnAccount.Services
{
    public class DbConnectorService
    {
        public string? connectionString;
        public DbConnectorService()
        {
            connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
        }
        /*
         * Gets a users email from thier login username
         */
        public string GetUserEmail(string usernameIn)
        {
            string? foundEmail = "";
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
         * Gets a users details from db given their email
         */
        public AppUserModel GetUserDetails(string userIn)
        {
            var foundUser = new AppUserModel();
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
                string command = "SELECT * FROM on_account.Users WHERE Id = @Id;";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@Id", userIn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    foundUser.Id = reader1.GetString(0);
                    foundUser.ScreenName = reader1.GetString(1);
                    foundUser.FirstName = reader1.GetString(2);
                    foundUser.LastName = reader1.GetString(3);
                    foundUser.PhoneNumber = reader1.GetString(4);
                    foundUser.DateofBirth = reader1.GetString(5);
                    foundUser.Address = reader1.GetString(6);
                    foundUser.City = reader1.GetString(7);
                    foundUser.State = reader1.GetString(8);
                    foundUser.Zip = reader1.GetString(9);
                    foundUser.UserRole = reader1.GetString(10);
                    foundUser.ActiveStatus = reader1.GetString(11);
                    foundUser.AcctSuspensionDate = reader1.GetString(12);
                    foundUser.AcctReinstatementDate = reader1.GetString(13);
                    foundUser.LastPasswordChangedDate = reader1.GetString(14);
                    foundUser.PasswordResetDays = reader1.GetString(15);
                    foundUser.UserName = reader1.GetString(16);
                    foundUser.NormalizedUserName = reader1.GetString(17);
                    foundUser.Email = reader1.GetString(18);
                    foundUser.ProfileImage = reader1.GetString(29);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            bool Succeeded = false;
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            var resultsList = new List<SelectListItem>();
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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

        /*
         * Gets a list of normal side options
         */
        public List<SelectListItem> GetNormalSideOptions()
        {
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            var resultsList = new List<SelectListItem>();
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            var resultsList = new List<PassHashModel>();
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
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


    }

}




