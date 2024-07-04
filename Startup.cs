using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using winfenixApi.Application.Interfaces;
using winfenixApi.Application.Services;
using winfenixApi.Core.Interfaces;
using winfenixApi.Core.Validators;
using winfenixApi.Infrastructure.Configurations;
using winfenixApi.Infrastructure.Data;
using winfenixApi.Infrastructure.Repositories;
using winfenixApi.Repositories;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDatabaseSettings(services);
        ConfigureControllers(services);
        ConfigureServicesAndRepositories(services);
        ConfigureValidators(services);
        ConfigureSwagger(services);
        ConfigureAuthentication(services);
    }

    private void ConfigureDatabaseSettings(IServiceCollection services)
    {
        services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
        services.AddSingleton<DatabaseContext>();
    }

    private void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers();
    }

    private void ConfigureServicesAndRepositories(IServiceCollection services)
    {
        services.AddScoped<IDynamicRepository, DynamicRepository>();
        services.AddScoped<IDynamicService, DynamicService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private void ConfigureValidators(IServiceCollection services)
    {
        var validators = new Dictionary<string, IValidator>
        {
            { "Product", new ProductValidator() }
        };
        services.AddSingleton<IDictionary<string, IValidator>>(validators);
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "winfenixApi", Version = "v1" });
        });
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        var key = Configuration["Jwt:KeySecret"];
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["Jwt:Authority"];
                options.RequireHttpsMetadata = false;
                options.Audience = "winfenixApi";
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            ConfigureDevelopmentEnvironment(app);
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void ConfigureDevelopmentEnvironment(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "winfenixApi v1"));
    }
}
