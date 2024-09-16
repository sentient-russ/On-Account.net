using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnAccount.Models
{
    public class PassHashModel
    {
        [Key]
        public int? Id { get; set; }
        public string? userId { get; set; }
        public string? passhash { get; set; }

    }
}
