using System.ComponentModel;

namespace oa.Models
{
    public class LogModel
    {
        public int? Id { get; set; }
        [DisplayName("Change Date:")]
        public DateTime? ChangeDate { get; set; }
        [DisplayName("Users Id:")]
        public string? UserId { get; set; }
        [DisplayName("Changed From:")]
        public string? ChangedFrom { get; set; }
        [DisplayName("Changed To:")]
        public string? ChangedTo { get; set; }
    }
}
