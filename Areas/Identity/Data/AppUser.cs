using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnAccount.Areas.Identity.Data;

public class AppUser : IdentityUser
{
    public string? ScreenName { get; set; } = "";
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public override string? PhoneNumber { get; set; } = "";
    internal string? DateofBirth { get; set; } = "";
    internal string? Address { get; set; } = "";
    public string? UserRole { get; set; } = "";
    public bool? ActiveStatus { get; set; } = true;

    //[System.ComponentModel.DataAnnotations.Schema.NotMapped]
    //public List<PostModel>? Posts { get; set; }

    //[System.ComponentModel.DataAnnotations.Schema.NotMapped]
}

