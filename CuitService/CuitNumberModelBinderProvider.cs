using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace CuitService
{
    public class CuitNumberModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(CuitNumber))
            {
                return new CuitNumberModelBinder();
            }

            return null;
        }
    }
}
