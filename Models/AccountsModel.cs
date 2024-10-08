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
    [ApiController]
    [BindProperties(SupportsGet = true)]
    public class AccountsModel

    {
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Account Name:")]
        public string? name { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = true)]
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
        public string? term { get; set; }

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
        public string? opening_transaction_num { get; set; }

        [Required]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Current Balance:")]
        public decimal? current_balance { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account created by:  (Auto assigned)")]
        public string? created_by { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Status:")]
        public string? account_status { get; set; }

        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Starting Balance:")]
        public decimal? starting_balance { get; set; }

        [Required]
        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime? transaction_1_date { get; set; } = System.DateTime.Today;

        [Required]
        [NotMapped]
        public string? transaction_1_dr_account { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_1_cr_account { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_2_dr_account { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_2_cr_account { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_1_dr { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_1_cr { get; set; }


        [Required]
        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime? transaction_2_date { get; set; } = System.DateTime.Today;

        [Required]
        [NotMapped]
        public string? transaction_2_dr { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_2_cr { get; set; }

        [Required]
        [NotMapped]
        public string? transaction_dr_total { get; set; }

        [Required]
        [NotMapped]

        public string? transaction_cr_total { get; set; }

        [ValidateNever]
        [NotMapped]
        public List<AccountsModel>? accounts_list { get; set; }

        [ValidateNever]
        [NotMapped]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Total:")]
        public string? total_adjustment { get; set; }

        [ValidateNever]
        [NotMapped]
        public string? error_state { get; set; }

        [NotMapped]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 0)]
        [DisplayName("Description:")]
        public string? transaction_1_description { get; set; } = "";

    }
}