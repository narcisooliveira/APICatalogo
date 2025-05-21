using System.Text;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using APICatalogo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(configure =>
    configure.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 

builder.Services.AddEndpointsApiExplorer();
// Add authorization button to the Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICatalogo", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, 
            []
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApiCatalogoContext>()
    .AddDefaultTokenProviders();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddDbContext<ApiCatalogoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var secretKey = builder.Configuration["Jwt:SecretKey"] 
          ?? throw new ArgumentNullException($"Invalid configuration value for 'Jwt:Secret'");

builder.Services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("MasterOnly", policy => policy.RequireRole("Master")
        .RequireClaim("Id", "narciso"))
    .AddPolicy("ExclusiveOnly", policy => policy.RequireRole("Exclusive")
        .RequireAssertion(context =>
        {
            var user = context.User;
            var hasClaim = user.HasClaim(c => 
                c is { Type: "Id", Value: "narciso" } || user.IsInRole("Master"));
            return hasClaim;
        }));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

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

loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
