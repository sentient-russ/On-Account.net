using OnAccount.Models;
using MySql.Data.MySqlClient;
using OnAccount.Areas.Identity.Data;
using OnAccount.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
                cmd1.Parameters.AddWithValue("@UserName", userIn.UserName);
                cmd1.Parameters.AddWithValue("@Email", userIn.Email);
                cmd1.Parameters.AddWithValue("@NormalizedUserName", userIn.NormalizedUserName);
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
                string command = "UPDATE on_account.Users SET LockoutEnd = @LoukoutEnd, LockoutEnabled = @LockoutEnabled, AcctSuspensionDate = @AcctSuspensionDate, AccountReinstatementDate = @AccountReinstatementDate WHERE id LIKE @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", userIn.Id);
                cmd1.Parameters.AddWithValue("@AcctSuspensionDate", userIn.AcctSuspensionDate);
                cmd1.Parameters.AddWithValue("@AcctReinstatementDate", userIn.AcctReinstatementDate);
                cmd1.Parameters.AddWithValue("@LockoutEnd", userIn.AcctReinstatementDate);
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

    }

}




