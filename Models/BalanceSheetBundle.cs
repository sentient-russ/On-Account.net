namespace oa.Models
{
    public class BalanceSheetBundle
    {
        public List<AccountsModel> ShortTermAssets { get; set; } = new List<AccountsModel>();
        public double ShortTermAssetsTotal { get; set; } = 0;
        public List<AccountsModel> LongTermAssets { get; set; } = new List<AccountsModel>();
        public double LongTermAssetsTotal { get; set; } = 0;
        public List<AccountsModel> ShortTermLiabilities { get; set; } = new List<AccountsModel>();
        public double ShortTermLiabilitiesTotal { get; set; } = 0;
        public List<AccountsModel> LongTermLiabilities { get; set; } = new List<AccountsModel>();
        public double LongTermLiabilitiesTotal { get; set; } = 0;
        public List<AccountsModel> Equities { get; set; } = new List<AccountsModel>();
        public double EquityTotal { get; set; } = 0;
        public double TotalLiabilitiesStockHolderEquity { get; set; } = 0;
    }
}