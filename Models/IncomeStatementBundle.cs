namespace oa.Models
{
    public class IncomeStatementBundle
    {
        public List<AccountsModel> RevenueAccountsList { get; set; } = new List<AccountsModel>();
        public double RevenueAccountsTotal { get; set; } = 0;
        public List<AccountsModel> ExpenseAccountsList { get; set; } = new List<AccountsModel>();
        public double ExxpenseAccountsTotal { get; set; } = 0;
        public double Net { get; set; } = 0;
    }
}
