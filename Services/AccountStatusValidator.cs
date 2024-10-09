using oa.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace oa.Services
{
    public class AccountStatusValidator : ValidationAttribute
    {
        private readonly string _balancePropertyName;
        private readonly double _requiredBalance;

        public AccountStatusValidator(string balancePropertyName, double requiredBalance)
        {
            _balancePropertyName = balancePropertyName;
            _requiredBalance = requiredBalance;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var balanceProperty = validationContext.ObjectType.GetProperty(_balancePropertyName);
            if (balanceProperty == null)
            {
                return new ValidationResult($"Unknown property: {_balancePropertyName}");
            }

            var balanceValue = balanceProperty.GetValue(validationContext.ObjectInstance);
            if (balanceValue == null || !(balanceValue is decimal))
            {
                return new ValidationResult($"Balance must be $0.00 to inactivate.");
            }

            var balance = (decimal)balanceValue;
            var status = value as string;
            if (status == "Inactive")
            {
                if(balance != (decimal)_requiredBalance)
                {
                    return new ValidationResult($"The balance must be {_requiredBalance} to deactivate.");
                }
                
            }
            return ValidationResult.Success;
        }
    }
}