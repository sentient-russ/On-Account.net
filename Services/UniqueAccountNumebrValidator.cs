using oa.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace oa.Services
{
    public class UniqueAccountNumberValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            DbConnectorService connectorService = new DbConnectorService();
            List<AccountsModel> accounts = connectorService.GetChartOfAccounts();
            if (accounts != null)
            {
                foreach (AccountsModel account in accounts)
                {
                    string str1 = account.number.ToString().ToLower();
                    string str2 = value.ToString().ToLower();
                    if (str1.Equals(str2))
                    {
                        return new ValidationResult("The account number must be unigue.");
                    }
                    bool failed = str2.Any(c => !char.IsDigit(c));
                    if (failed)
                    {
                        return new ValidationResult("The account number can only contain digits.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
