using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using winfenixApi.Application.Interfaces;
using winfenixApi.Application.Services;
using winfenixApi.Core.Interfaces;
using winfenixApi.Core.Validators;
using winfenixApi.Infrastructure.Configurations;
using winfenixApi.Infrastructure.Data;
using winfenixApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var configurationGlobal = builder.Configuration;
var configurationJWT = configurationGlobal.GetSection("JWT");

var loggerFactory = LoggerFactory.Create(builder =>
{
    //builder.AddEventLog(); // Puedes agregar otros proveedores de registro si es necesario
    builder.AddConsole(); // Puedes agregar otros proveedores de registro si es necesario
});

var _logger = loggerFactory.CreateLogger<Program>();
_logger.LogInformation("Realizando validaciones previas...");




// Add services to the container
builder.Services.AddControllers();
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<IDynamicRepository, DynamicRepository>();
builder.Services.AddScoped<IDynamicService, DynamicService>();

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register the validators dictionary
builder.Services.AddSingleton<IDictionary<string, IValidator>>(provider => new Dictionary<string, IValidator>());


// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

string issuer = jwtSettings.GetValue<string>("Issuer") ?? throw new ArgumentNullException("JwtSettings:Issuer");
string audience = jwtSettings.GetValue<string>("Audience") ?? throw new ArgumentNullException("JwtSettings:Audience");
string key = jwtSettings.GetValue<string>("KeySecret") ?? throw new ArgumentNullException("JwtSettings:KeySecret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// Configure other services, e.g., logging, authentication, etc.
builder.Services.AddLogging();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "winfenixApi",
        Version = "v1",
        Description = "Api de Winfenix Yelser",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                      });
});

// yycc
// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

