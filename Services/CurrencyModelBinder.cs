
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

namespace oa.Services
{
    public class CurrencyModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }
            string currencyString = value.ToString();
            currencyString = currencyString.Replace("$", "");
            currencyString = currencyString.Replace(",", "");

            if (decimal.TryParse(currencyString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid currency format.");
            }

            return Task.CompletedTask;
        }
    }

}
