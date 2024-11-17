using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using oa.Services;
using System.ComponentModel;

namespace oa.Models
{
    public class CurrentRaitoModel
    {
        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Current Assets Balance:")]
        public decimal? current_assets_balance { get; set; }

        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Current Liabilities Balance:")]
        public decimal? current_liabilities_balance { get; set; }
    }
}
