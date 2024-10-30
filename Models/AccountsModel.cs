using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using oa.Services;
using System.Configuration;
using System.Transactions;
/*
 This model serves as the primary module for working with the accounting system accounts.
 The fields reflect all attributes in the on_account.account database table.
 */
namespace oa.Models
{
    [BindProperties(SupportsGet = true)]
    public class AccountsModel : IValidatableObject

    {
        private const string CurrentBalancePropertyName = "current_balance";
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [UniqueAccountNameValidator]
        [DisplayName("Account Name:")]
        public string? name { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = true)]
        [UniqueAccountNumberValidator]
        [DisplayName("Account Number:")]
        public int? number { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = true)]
        [DisplayName("Sort priority:")]
        public int? sort_priority { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Normal side:")]
        public string? normal_side { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(1000, MinimumLength = 1)]
        [DisplayName("Description:")]
        public string? description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Type:")]
        public string? type { get; set; }

        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 0)]
        [DisplayName("Term:")]
        public string? term { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account statement type:")]
        public string? statement_type { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Creation Date:")]
        public DateTime? account_creation_date { get; set; } = System.DateTime.Now;

        [DisplayName("Opening Transaction Number: (Auto assigned)")]
        public string? opening_transaction_num { get; set; } = "";

        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Current Balance:")]
        public decimal? current_balance { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account created by:  (Auto assigned)")]
        public string? created_by { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [AccountStatusValidator(CurrentBalancePropertyName, 0.00)]
        [DisplayName("Status:")]
        public string? account_status { get; set; }

        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Starting Balance:")]
        public decimal? starting_balance { get; set; } = 0;

        [Required]
        [NotMapped]
        [ValidateNever]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? transaction_1_date { get; set; } = System.DateTime.Today;

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_1_dr_account { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_1_cr_account { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_2_dr_account { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_2_cr_account { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_1_dr { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_1_cr { get; set; }


        [Required]
        [NotMapped]
        [ValidateNever]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? transaction_2_date { get; set; } = System.DateTime.Today;

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_2_dr { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_2_cr { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_dr_total { get; set; }

        [Required]
        [NotMapped]
        [ValidateNever]
        public string? transaction_cr_total { get; set; }


        [ValidateNever]
        [NotMapped]
        public List<AccountsModel>? accounts_list { get; set; }

        [ValidateNever]
        [NotMapped]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Total:")]
        public string? total_adjustment { get; set; } = "$0.00";

        [ValidateNever]
        [NotMapped]
        public string? error_state { get; set; }

        [NotMapped]
        [ValidateNever]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 0)]
        [DisplayName("Description:")]
        public string? transaction_1_description { get; set; } = "";

        [NotMapped]
        [ValidateNever]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DisplayName("Journal Date:")]
        public DateTime? journal_date { get; set; } = DateTime.Today;

        [NotMapped]
        [ValidateNever]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 0)]
        [DisplayName("Journal Description:")]
        public string? journal_description { get; set; } = "";

        [NotMapped]
        [ValidateNever]
        public List<TransactionModel>? transactions_list { get; set; }

        [NotMapped]
        [ValidateNever]
        [DisplayName("Journal Id:")]
        public int? journal_id { get; set; }

        [NotMapped]
        [ValidateNever]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Status:")]
        public string? status { get; set; } = "Pending";

        [DataType(DataType.Text)]
        [StringLength(300, MinimumLength = 1)]
        [DisplayName("Comments:")]
        public string? comments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }

    }

}