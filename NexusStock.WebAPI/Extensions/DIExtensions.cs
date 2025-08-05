using NexusStock.BLL.Logic;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.DAL.Repository;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.WebAPI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection ConfigureBLLDependencies(this IServiceCollection services)
        {
            services.AddScoped<IClientLogic, ClientLogic>();
            services.AddScoped<IResourceLogic, ResourceLogic>();
            services.AddScoped<IUnitLogic, UnitLogic>();
            services.AddScoped<IReceiptLogic, ReceiptLogic>();
            services.AddScoped<IShipmentLogic, ShipmentLogic>();
            services.AddScoped<IStockLogic, StockLogic>();

            return services;
        }

        public static IServiceCollection ConfigureDALDependencies(this IServiceCollection services)
        {
            services.AddScoped<INexusUnitOfWork, NexusUnitOfWork>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IReceiptDocumentRepository, ReceiptDocumentRepository>();
            services.AddScoped<IShipmentDocumentRepository, ShipmentDocumentRepository>();

            return services;
        }
    }
}
