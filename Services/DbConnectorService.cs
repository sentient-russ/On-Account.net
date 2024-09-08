using OnAccount.Models;
using MySql.Data.MySqlClient;
using OnAccount.Areas.Identity.Data;

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
         * Updates the database to enable scheduled user lockout events
         */
        public void UpdateLockout(AppUser userIn)
        {
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            try
            {
                MySqlConnection conn1 = new MySqlConnection(connectionString);
                string command = "UPDATE on_account.Users SET LockoutEnd = @LoukoutEnd, LockoutEnabled = @LockoutEnabled WHERE id LIKE @id";
                conn1.Open();
                MySqlCommand cmd1 = new MySqlCommand(command, conn1);
                cmd1.Parameters.AddWithValue("@id", userIn.Id);
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




