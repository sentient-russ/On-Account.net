using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oa.Models
{
    public class ErrorModel
    {
        public ErrorModel() {
        }

        [Key]
        public int? Id { get; set; }
        [NotMapped]
        public string? IdStr { get; set; }
        public string? Error { get; set; }
        public string? Descritpion { get; set; }
    }
}
