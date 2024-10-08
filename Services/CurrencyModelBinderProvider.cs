using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace oa.Services
{
    public class CurrencyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(decimal))
            {
                return new CurrencyModelBinder();
            }

            return null;
        }
    }
}
