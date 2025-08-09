using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NexusStock.WebAPI.ModelBinders
{
    public class DateTimeOffsetModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            var dateString = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(dateString))
                return Task.CompletedTask;

            // Парсинг с автоматическим UTC
            if (DateTimeOffset.TryParse(
                dateString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.TryAddModelError(
                modelName,
                $"Invalid DateTimeOffset format: '{dateString}'");

            return Task.CompletedTask;
        }
    }
}
