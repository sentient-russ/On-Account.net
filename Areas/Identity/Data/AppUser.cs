using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnAccount.Areas.Identity.Data
{
    [ApiController]
    [Route("[Controller]")]
    [BindProperties(SupportsGet = true)]
    public class AppUser : IdentityUser
    {
        public string? ScreenName { get; set; } = "";
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public override string? PhoneNumber { get; set; } = "";
        internal string? DateofBirth { get; set; } = "";
        internal string? Address { get; set; } = "";
        internal string? City { get; set; } = "";
        internal string? State { get; set; } = "";
        internal string? Zip { get; set; } = "";
        public string? UserRole { get; set; } = "";
        public bool? ActiveStatus { get; set; } = true;

        public DateTime? AcctSuspensionDate { get; set; }

        public DateTime? AcctReinstatementDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string PasswordResetDays { get; set; } = "90";

        [NotMapped]
        public IEnumerable<SelectListItem>? RoleList { get; set; }


    }

}