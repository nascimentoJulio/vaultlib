using System.Net.Http.Headers;
using VautlLib;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
using HttpClient client = new();

client.BaseAddress = new Uri(configuration["VAULT_BASE_URL"]);
client.DefaultRequestHeaders.Add("X-Vault-Token", configuration["VAULT_TOKEN"]);

services.AddScoped<IVaultHandler>(sp => new VaultHandler(client));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
