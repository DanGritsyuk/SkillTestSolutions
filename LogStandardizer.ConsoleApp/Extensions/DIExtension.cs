using LogStandardizer.BLL.Logic.Contracts;
using LogStandardizer.BLL.Logic.Parsers;
using LogStandardizer.BLL.Logic;
using LogStandardizer.IO.FileServices;
using LogStandardizer.IO.FileServices.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LogStandardizer.ConsoleApp.Extensions
{
    public static class DIExtension
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Регистрация сервиса
            services.AddTransient<ILogParser<SimpleFormatParser>, SimpleFormatParser>();
            services.AddTransient<ILogParser<DetailedFormatParser>, DetailedFormatParser>();
            services.AddTransient<ILogStandardizer, LogStandardizerService>();

            services.AddSingleton<Application>();
            services.AddSingleton<ILogReader, FileLogReader>();
            services.AddSingleton<ILogWriter, FileLogWriter>();

            return services;
        }
    }
}
