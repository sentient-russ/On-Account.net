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

        [DisplayName("Last Opening/Closing Date:")]
        public DateTime? open_close_date { get; set; }

        [DisplayName("Last Opened/Closed By:")]
        public string? closing_user { get; set; }

        [DisplayName("Opened/Closed On Date:")]
        public DateTime? open_close_on_date { get; set; }

    }
}
