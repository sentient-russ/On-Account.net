﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace oa.Areas.Identity.Data
{
    [ApiController]
    [Route("[Controller]")]
    [BindProperties(SupportsGet = true)]
    public class AppUser : IdentityUser
    {

        static DateTime today = System.DateTime.Now;
        static DateTime birthdayPlaceholder = today.AddYears(-100);

        public string? ScreenName { get; set; } = "";
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public override string? PhoneNumber { get; set; } = "";
        public DateTime? DateofBirth { get; set; } = birthdayPlaceholder;
        public string? Address { get; set; } = "";
        public string? City { get; set; } = "";
        public string? State { get; set; } = "";
        public string? Zip { get; set; } = "";
        public string? UserRole { get; set; } = "";
        public string? ActiveStatus { get; set; } = "Active";

        public DateTime? AcctSuspensionDate { get; set; }

        public DateTime? AcctReinstatementDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string PasswordResetDays { get; set; } = "90";

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
        public string? ProfileImage { get; set; } = "";


        [NotMapped]
        [ValidateNever]
        public List<String>? AdminManagerEmails { get; set; }
    }

}