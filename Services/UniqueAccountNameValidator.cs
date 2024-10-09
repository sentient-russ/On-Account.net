using System.ComponentModel.DataAnnotations;
using System;
using oa.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using oa.Services;

namespace oa.Services
{
    public class UniqueAccountNameValidator : ValidationAttribute
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
                    string str1 = account.name.ToLower();
                    string str2 = value.ToString().ToLower();
                    if (str1.Equals(str2))
                    {
                        return new ValidationResult("The account name must be unigue.");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
