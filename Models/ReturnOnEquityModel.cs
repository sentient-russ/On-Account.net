using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using oa.Services;
using System.ComponentModel;

namespace oa.Models
{
    public class ReturnOnEquityModel
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
        [DisplayName("Equity Balance:")]
        public decimal? equity_balance { get; set; }
    }
}
