using Microsoft.Extensions.DependencyInjection;
using StringCompressor.ConsoleApp;
using StringCompressor.ConsoleApp.Extensions;

var services = new ServiceCollection();
services.ConfigureServices();
using var serviceProvider = services.BuildServiceProvider();

var app = serviceProvider.GetRequiredService<Application>();
await app.RunAsync();