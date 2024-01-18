using APICatalogo.Context;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(configure =>
    configure.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddDbContext<ApiCatalogoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IMeuServico, MeuServico>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

app.ConfigureExceptionHandler(loggerFactory);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
