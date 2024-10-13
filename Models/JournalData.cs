namespace oa.Models
{
    public class JournalData
    {
        public string JournalId { get; set; }
        public string UserName { get; set; }
        public string JournalDate { get; set; }
        public decimal JournalTotal { get; set; }
        public string JournalStatus { get; set; }
        public string JournalNotes { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        public string TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public List<LineData> Lines { get; set; }
    }

    public class LineData
    {
        public string DrAccount { get; set; }
        public string CrAccount { get; set; }
        public string PostRef { get; set; }
        public decimal DrAmount { get; set; }
        public decimal CrAmount { get; set; }
    }
}
