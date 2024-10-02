using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;

namespace oa.Models
{
    public class TransactionModel
    {
        public int? id { get; set; }

        [IntegerValidator(MinValue = 1, MaxValue = 10000, ExcludeRange = true)]
        [DisplayName("Journal Id:")]
        public int? journal_id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Debit Account:")]
        public string? debit_account { get; set; }

        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Dr.")]
        public int? debit_amount { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Credit Account")]
        public string? credit_account { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Cr.")]
        public int? credit_amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Transaction date:")]
        public int? transaction_date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account created by:")]
        public string? created_by { get; set; }

    }
}
