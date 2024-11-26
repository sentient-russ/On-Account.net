using System.ComponentModel.DataAnnotations;

namespace oa.Models
{
    public class ErrorModel
    {
        [Key]
        public int? Id { get; set; }
        public string? Error { get; set; }
        public string? Descritpion { get; set; }
    }
}
