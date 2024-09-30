using System.ComponentModel.DataAnnotations;

namespace oa.Models
{
    /*
     * Class used to pupulate account type options lists. 
     * Options stored in on_account.account_normal_side_type and - 
     * may be obtained with DbConnectorService.GetNormalSideOptions()
     */
    public class AccountTypeModel
    { 
        public int? id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        public string? account_option { get; set; }
    }
}
