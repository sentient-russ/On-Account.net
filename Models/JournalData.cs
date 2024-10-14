using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace oa.Models
{
    public class JournalEntry
    {
        [JsonProperty("journal_id")]
        public int JournalId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("journal_date")]
        public string JournalDate { get; set; } // Use string if the date format is non-standard

        [JsonProperty("journal_total")]
        public decimal? JournalTotal { get; set; }

        [JsonProperty("journal_status")]
        public string JournalStatus { get; set; }

        [JsonProperty("journal_notes")]
        public string JournalNotes { get; set; }

        [JsonProperty("transactions")]
        public List<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("data_transaction")]
        public int DataTransaction { get; set; }

        [JsonProperty("transaction_description")]
        public string TransactionDescription { get; set; }

        [JsonProperty("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [JsonProperty("transaction_upload")]
        public string TransactionUpload { get; set; }

        [JsonProperty("line_items")]
        public List<LineItem> LineItems { get; set; }
    }

    public class LineItem
    {
        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("dr_account")]
        public string DrAccount { get; set; }

        [JsonProperty("cr_account")]
        public string CrAccount { get; set; }

        [JsonProperty("post_ref")]
        public string PostRef { get; set; }

        [JsonProperty("dr_amount")]
        public decimal DrAmount { get; set; }

        [JsonProperty("cr_amount")]
        public decimal CrAmount { get; set; }
    }
}