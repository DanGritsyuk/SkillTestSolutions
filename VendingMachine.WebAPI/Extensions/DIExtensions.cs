using VendingMachine.BLL.Logic;
using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.DAL.Repository;
using VendingMachine.DAL.Repository.Contracts;

namespace VendingMachine.WebAPI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection ConfigureBLLDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDrinkLogic, DrinkLogic>();

            return services;
        }

        public static IServiceCollection ConfigureDALDependencies(this IServiceCollection services)
        {
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IDrinksRepository, DrinksRepository>();
            services.AddScoped<ICoinRepository, CoinRepository>();

            return services;
        }
    }
}
