using LogStandardizer.ConsoleApp;
using LogStandardizer.ConsoleApp.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.ConfigureServices();
using var serviceProvider = services.BuildServiceProvider();

var app = serviceProvider.GetRequiredService<Application>();
await app.RunAsync();