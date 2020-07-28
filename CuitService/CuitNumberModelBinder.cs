using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace CuitService
{
    public class CuitNumberModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Task.CompletedTask;
            }

            var errorMessage = CuitNumber.ValidateNumber(value);

            if (errorMessage != null)
            {
                bindingContext.ModelState.TryAddModelError(
                    modelName,
                    errorMessage.ErrorMessage);
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(new CuitNumber(value));
            return Task.CompletedTask;
        }
    }

}
