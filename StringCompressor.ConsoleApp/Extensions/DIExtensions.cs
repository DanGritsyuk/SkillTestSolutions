using Microsoft.Extensions.DependencyInjection;
using StringCompressor.BLL.Logic;
using StringCompressor.BLL.Logic.Contracts;
using StringCompressor.DAL.Repository;
using StringCompressor.DAL.Repository.Contracts;

namespace StringCompressor.ConsoleApp.Extensions
{
    internal static class DIExtensions
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services)
        {

            // Настройка репозитория файла для записи строки
            IFileContext txtFileData = new TextFileContext(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "compressed.txt"));

            // Регистрация сервиса
            services.AddTransient<IStringCompressorService, StringCompressorService>();
            services.AddTransient<ICompressionFacade, CompressionFacade>();

            // Регистрация основных узлов
            services.AddSingleton<Application>();
            services.AddSingleton(txtFileData);
            services.AddSingleton<IFileRepository, FileRepository>();

            return services;
        }
    }
}
