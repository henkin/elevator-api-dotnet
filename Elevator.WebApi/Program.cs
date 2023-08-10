using System.Text.Json.Serialization;
using Elevator.WebApi.Controllers;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

// Bootstrap Serilog logger before creating the host
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Bootstrap pt 2: Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services), 
        writeToProviders: true);
    
    // Add services to the container.
    builder.Services.AddControllers()
        // Allow enums to be serialized as strings
        .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

    builder.Services.AddSingleton<IFloorRequestService, FloorRequestService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(ConfigureSwagger());

    var app = builder.Build();
    app.UseSerilogRequestLogging(); // Optional: Log HTTP requests
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

Action<SwaggerGenOptions> ConfigureSwagger()
{
    return c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Elevator API",
            Version = "v1",
            Description = "API for managing elevator floor requests",
            Contact = new OpenApiContact
            {
                Name = "Paul Henkin",
                Email = "talktopaul@gmail.com",
            },
        });

        // // Add XML comments for better documentation
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    };
}

/// <summary>
/// A work-around for the lack of a Program class in .NET 6 minimal APIs.
/// We need it to be able to use the WebApplicationBuilder.
/// </summary>
public partial class Program { } 
