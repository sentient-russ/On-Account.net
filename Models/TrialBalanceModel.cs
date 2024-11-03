using Microsoft.AspNetCore.Mvc;
using oa.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace oa.Models
{
    public class TrialBalanceModel
    {
        [Key]
        public string? accountname{ get; set; }

        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Debit:")]
        public double? debit { get; set; }


        [ModelBinder(BinderType = typeof(CurrencyModelBinder))]
        [IntegerValidator(MinValue = 1, MaxValue = 1000000000, ExcludeRange = true)]
        [DisplayName("Credit:")]
        public double? credit {  get; set; }
    }
}
