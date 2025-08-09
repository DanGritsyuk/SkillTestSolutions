using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NexusStock.WebAPI.ModelBinders
{
    public class DateTimeOffsetModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTimeOffset) ||
                context.Metadata.ModelType == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetModelBinder();
            }
            return null;
        }
    }
}
