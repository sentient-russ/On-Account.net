using System.ComponentModel.DataAnnotations;

namespace oa.Models
{
    public class SupportingDocumentsModel
    {
        [Key]
        public int id { get; set; }

        public int? journal_id { get; set; }

        public string? file_name { get; set; }
    }
}
