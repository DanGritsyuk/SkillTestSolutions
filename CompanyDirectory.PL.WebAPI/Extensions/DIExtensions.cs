using CompanyDirectory.BLL.Logic;
using CompanyDirectory.BLL.Logic.Contracts;
using CompanyDirectory.DAL.Repository;
using CompanyDirectory.DAL.Repository.Contracts;

namespace CompanyDirectory.PL.WebAPI.Extensions
{
    internal static class DIExtensions
    {
        public static IServiceCollection ConfigureBLLDependencies(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            return services;
        }

        public static IServiceCollection ConfigureDALDependencies(this IServiceCollection services)
        {
            services.AddScoped(provider =>
                provider.GetRequiredService<IDBConnection>().CreateConnectionAsync().Result);

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            return services;
        }
    }
}