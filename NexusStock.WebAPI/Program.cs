using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusStock.DAL.Repository;
using NexusStock.WebAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NexusStock.WebAPI.Mapping;

namespace NexusStock.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Регистрация AutoMapper через стандартный метод
            builder.Services.AddAutoMapper(cfg => { }, typeof(ClientProfile).Assembly);

            builder.Services.ConfigureDALDependencies();
            builder.Services.ConfigureBLLDependencies();

            string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
            builder.Services.AddDbContext<NexusStockDbContext>(options =>
                options.UseNpgsql(connection));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                // Проверка конфигурации AutoMapper
                using var scope = app.Services.CreateScope();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                try
                {
                    //Проверка валидности конфигурации
                    ((MapperConfiguration)mapper.ConfigurationProvider).AssertConfigurationIsValid();
                    Console.WriteLine("AutoMapper configuration is valid");
                }
                catch (AutoMapperConfigurationException ex)
                {
                    Console.WriteLine("AutoMapper configuration error:");
                    Console.WriteLine(ex.Message);
                }

                // Предварительная компиляция маппингов
                ((MapperConfiguration)mapper.ConfigurationProvider).CompileMappings();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}