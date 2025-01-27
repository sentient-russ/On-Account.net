using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

/*
 This model serves as the primary module for housing system user information.  
 The fields reflect all attributes in the Users database table.
 Changes here may also impact the identity authentication and authorization process - 
 and will be included in the database midgrations process that changes the users table.
 */
namespace oa.Models
{
    [ApiController]
    [BindProperties(SupportsGet = true)]
    public class AppUserModel
    {
        static DateTime today = System.DateTime.Now;
        static DateTime birthdayPlaceholder = today.AddYears(-100);
        static string beginingDateFormatted = birthdayPlaceholder.ToString("yyyyMMdd");
        static string todayDateFormatted = today.ToString("yyyyMMdd");

        public string? Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("User Name:")]
        public string? ScreenName { get; set; } = "";
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("First Name:")]
        public string? FirstName { get; set; } = "";
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Last Name:")]
        public string? LastName { get; set; } = "";
        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 12)]
        [DisplayName("Phone Number:")]
        public string? PhoneNumber { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(150, MinimumLength = 1)]
        [DisplayName("Address:")]
        public string? Address { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("City:")]
        public string? City { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(2, MinimumLength = 2)]
        [DisplayName("State:")]
        public string? State { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, MinimumLength = 1)]
        [DisplayName("Zip:")]
        public string? Zip { get; set; } = "";

        [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Birthday:")]
        public DateTime? DateofBirth { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("User Role:")]
        public string? UserRole { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, MinimumLength = 1)]
        [DisplayName("Active Status:")]
        public string? ActiveStatus { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, MinimumLength = 1)]
        [DisplayName("User Name:")]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(255, MinimumLength = 1)]
        [DisplayName("Email Address:")]
        public string? Email { get; set; }

        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Normalized User Name:")]
        public string? NormalizedUserName { get; set; }

        [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Suspension Date:")]
        public DateTime? AcctSuspensionDate { get; set; }

        [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Reinstatement Date:")]
        public DateTime? AcctReinstatementDate { get; set; }

        [DisplayFormat(DataFormatString = "{:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Last Password Change:")]
        public string? LastPasswordChangedDate { get; set; }

        
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        [DisplayName("Next Reset Days:")]
        public string? PasswordResetDays { get; set; } = "90";

        [ValidateNever]
        [NotMapped]
        public IEnumerable<SelectListItem>? RoleList { get; set; }

        [NotMapped]
        [DataType(DataType.Text)]
        [StringLength(500, MinimumLength = 1)]
        [DisplayName("Subject:")]
        public string? Subject { get; set; }

        [NotMapped]
        [DataType(DataType.Text)]
        [StringLength(500, MinimumLength = 1)]
        [DisplayName("Message:")]
        public string? Message { get; set; }

        [DataType(DataType.Text)]
        [StringLength(500, MinimumLength = 1)]
        public string? ProfileImage { get; set; }

    }

}