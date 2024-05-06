using Logger;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Register Logger
// Register Serilog logger with ASP.NET Core logging
builder.Host.UseSerilog();

var levelSwitch = new LoggingLevelSwitch();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.FromLogContext() // Enrich log events with context information
    .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName) // Include the application name
    .Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName) // Include the environment name
    .WriteTo.Console() // Write logs to console
    .WriteTo.Seq("http://localhost:5341",
                 apiKey: "FcUQ9ujzJGvFK7gvRkrj",
                 controlLevelSwitch: levelSwitch)
    .CreateLogger();

Log.Information("Starting up");
#endregion

#region Register Dependencies
builder.Services.AddTransient<ILoggerService, LoggerService>();
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

app.Run();
