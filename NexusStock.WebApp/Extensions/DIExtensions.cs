using NexusStock.WebApp.BLL.Services;
using NexusStock.WebApp.BLL.Services.Contracts;

namespace NexusStock.WebApp.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection ConfigureBLLDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitsService, UnitsService>();
            services.AddScoped<IResourcesService, ResourcesService>();
            services.AddScoped<IClientsService, ClientsService>();

            return services;
        }
    }
}
