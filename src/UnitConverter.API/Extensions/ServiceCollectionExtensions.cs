using UnitConverter.API.Services;

namespace UnitConverter.API.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IConversionService, ConversionService>();
        return services;
    }

    /// <summary>
    /// Registers and configures Swagger / OpenAPI.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title       = "Unit Converter API",
                Version     = "v1",
                Description = "A RESTful API for converting values between units of measurement. " +
                              "Supports Length, Mass, Temperature, Speed, Area, Volume, Pressure, Energy, and Data.",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Unit Converter Team"
                }
            });

            // Include XML comments from the API project
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}
