using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using oa.Services;



/*
 This model serves as the primary module for working with the accounting system accounts.
 The fields reflect all attributes in the accounting database table.
 */
namespace oa.Models
{
    public class AccountsModel

    {
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Account Name:")]
        public string? account_name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Normal Side:")]
        public string? account_normal_side { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Account Type:")]
        public string? account_type { get; set; }

        [ValidateNever]
        [NotMapped]
        public IEnumerable<SelectListItem>? normal_side_options_list { get; set; }

        [ValidateNever]
        [NotMapped]
        public IEnumerable<SelectListItem>? account_type_options_list { get; set; }

        


    }
}