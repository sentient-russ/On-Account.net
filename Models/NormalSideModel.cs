using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace oa.Models
{
    public class NormalSideModel
    {
        public int? id { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        public string? side_option { get; set; }
    }
}
