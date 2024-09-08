using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnAccount.Models
{
    public class AppUserModel
    {
        public string? ScreenName { get; set; } = "";
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public string? PhoneNumber { get; set; } = "";
        internal string? DateofBirth { get; set; } = "";
        internal string? Address { get; set; } = "";
        public string? UserRole { get; set; } = "";
        public bool? ActiveStatus { get; set; } = true;

        public string? UserName { get; set; }
        public string? Email {  get; set; }

        public string? NormalizedUserName { get; set; }

        public DateTime? AcctSuspensionDate { get; set; }

        public DateTime? AcctReinstatementDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string PasswordResetDays { get; set; } = "90";


    }
}