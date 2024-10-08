using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;

namespace oa.Models
{
    public class TransactionModel
    {
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Debit Account:")]
        public int? debit_account { get; set; }

        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Dr.")]
        public double? debit_amount { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Credit Account")]
        public int? credit_account { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Cr.")]
        public double? credit_amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Transaction date:")]
        public DateTime? transaction_date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Created by:")]
        public string? created_by { get; set; }

        [DisplayName("Opening transaction:")]
        public bool? is_opening { get; set; } = false;

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Status:")]
        public string? status { get; set; } = "Pending";

        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 0)]
        [DisplayName("Description:")]
        public string? description { get; set; } = "";

    }
}
