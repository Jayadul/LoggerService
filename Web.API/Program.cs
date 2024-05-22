using Logger;
using LogTracker;
using PeriodicLogger;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);
// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

#region Register Logger
//// Register Serilog logger with ASP.NET Core logging
//builder.Host.UseSerilog();

//var levelSwitch = new LoggingLevelSwitch();

//builder.Services.AddSerilog((services, lc) => lc
//    .MinimumLevel.ControlledBy(levelSwitch)
//    .Enrich.FromLogContext() // Enrich log events with context information
//    .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName) // Include the application name
//    .Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName) // Include the environment name
//    .WriteTo.Console() // Write logs to console
//    .WriteTo.Seq(serverUrl: builder.Configuration["Seq:SeqServerUrl"]!, // Use null-forgiving operator
//                 apiKey: builder.Configuration["Seq:SeqApiKey"]!,
//                 controlLevelSwitch: levelSwitch)
//    .AddSeliseLogTracker(builder.Configuration, services)
//    );
// Create a level switch for controlling logging levels
var levelSwitch = new LoggingLevelSwitch();

// Configure Serilog for real-time file logging with date in file name
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
    .Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Real-time file logging with date
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);
#endregion

#region Register Dependencies
//builder.Services.AddTransient<ILoggerService, LoggerService>();

builder.Services.AddHostedService<PeriodicLoggerService>(); // Register the background service for periodic logging
builder.Services.AddSingleton<LoggingLevelSwitch>(); // Register LoggingLevelSwitch as a singleton service

#endregion

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

// register the LogBeeMiddleware just before the app.Run()
app.UseSeliseLogTrackerMiddleware();

app.Run();
