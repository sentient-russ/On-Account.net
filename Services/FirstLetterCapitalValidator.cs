using System;
using System.ComponentModel.DataAnnotations;

namespace OnAccount.Services
{
    public class FirstLetterCapitalValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string password = value.ToString();

            if (string.IsNullOrEmpty(password) || !char.IsUpper(password[0]))
            {
                return new ValidationResult("The first letter of the password must be capitalized.");
            }

            return ValidationResult.Success;
        }
    }
}