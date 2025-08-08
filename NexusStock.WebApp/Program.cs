using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexusStock.WebApp.Extensions;

namespace NexusStock.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.ConfigureBLLDependencies();

            // Получаем базовый URL API из конфигурации
            //var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
            //    ?? throw new InvalidOperationException("ApiBaseUrl not found in appsettings.json");

            // Регистрируем HttpClient с адресом API
            //builder.Services.AddScoped(sp => new HttpClient
            //{
            //    BaseAddress = new Uri(apiBaseUrl)
            //});

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7140/api/v1/")
            });



            await builder.Build().RunAsync();
        }
    }
}
