using CompanyDirectory.DAL.Repository;
using CompanyDirectory.DAL.Repository.Contracts;
using CompanyDirectory.PL.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddSingleton<IDBConnection>(provider =>
    new DbConnection(
        connection,
        provider.GetRequiredService<ILogger<DbConnection>>()));

builder.Services.ConfigureDALDependencies();
builder.Services.ConfigureBLLDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
