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


    }
}
