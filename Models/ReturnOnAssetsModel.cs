using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using oa.Services;
using System.ComponentModel;

namespace oa.Models
{
    public class ReturnOnAssetsModel
    {
        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Revenue Balance:")]
        public decimal? revenues_balance { get; set; }

        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Expenses Balance:")]
        public decimal? expenses_balance { get; set; }

        [ValidateNever]
        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [DisplayName("Current Assets Balance:")]
        public decimal? assets_balance { get; set; }
    }
}
