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
    public class AccountsModel

    {
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Acct. Name:")]
        public string? name { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = true)]
        [DisplayName("Acct. Number:")]
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

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Term:")]
        public string? term { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account statement type:")]
        public string? statement_type { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Account created on:")]
        public int? account_creation_date { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 10000000, ExcludeRange = true)]
        [DisplayName("Starting balance:")]
        public int? opening_transaction_num { get; set; }

        [Required]
        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Current Balance:")]
        public int? current_balance { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account created by:")]
        public string? created_by { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Status:")]
        public string? account_status { get; set; }

        [ValidateNever]
        [NotMapped]
        public IEnumerable<SelectListItem>? normal_side_options_list { get; set; }

        [ValidateNever]
        [NotMapped]
        public IEnumerable<SelectListItem>? account_type_options_list { get; set; }

        


    }
}