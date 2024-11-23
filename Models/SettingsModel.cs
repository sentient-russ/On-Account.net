using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace oa.Models
{
    public class SettingsModel
    {
        [Key]
        public int? Id { get; set; }

        [DisplayName("Business Name:")]
        public string? business_name { get; set; }

        [DisplayName("Last Closing Date:")]
        public DateTime? open_close_date { get; set; }

        [DisplayName("Last Closed By:")]
        public string? closing_user { get; set; }

        [DisplayName("User closed On Date:")]
        public DateTime? open_close_on_date { get; set; }

        [DisplayName("Accounting System Start Date:")]
        public DateTime? system_start_date { get; set; }

    }
}
