using LogTracker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace PeriodicLogger;

public class PeriodicLoggerService : BackgroundService
{
    private readonly ILogger<PeriodicLoggerService> _logger; // Logger instance
    private readonly IConfiguration _configuration; // Configuration instance
    private readonly IServiceProvider _services; // Service provider instance
    private readonly LoggingLevelSwitch _levelSwitch; // Logging level switch instance
    private long _lastReadPosition = 0; // Tracks the last read position in the log file
    private DateTime _lastResetDate = DateTime.UtcNow.Date; // Tracks the date of the last reset

    public PeriodicLoggerService(
        ILogger<PeriodicLoggerService> logger,
        IConfiguration configuration,
        IServiceProvider services,
        LoggingLevelSwitch levelSwitch)
    {
        _logger = logger; // Injected logger
        _configuration = configuration; // Injected configuration
        _services = services; // Injected service provider
        _levelSwitch = levelSwitch; // Injected logging level switch
    }

    // Main execution method for the background service
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                SyncLogs(); // Sync logs periodically
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while syncing logs"); // Log any errors
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait for 5 minutes before the next sync

            // Reset the last read position to 0 if a new day has started since the last reset
            if (DateTime.UtcNow.Date != _lastResetDate)
            {
                _lastResetDate = DateTime.UtcNow.Date; // Update the last reset date
                _lastReadPosition = 0; // Reset the last read position to 0
            }
        }
    }

    // Method to sync logs from the file
    private void SyncLogs()
    {
        var logFilePath = $"logs/log-{DateTime.UtcNow:yyyyMMdd}.txt"; // Get the log file path with the current date
        if (File.Exists(logFilePath))
        {
            var logEntries = new List<string>(); // List to hold log entries
            using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                fileStream.Seek(_lastReadPosition, SeekOrigin.Begin); // Seek to the last read position
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    logEntries.Add(line); // Add each log entry to the list
                }
                _lastReadPosition = fileStream.Position; // Update the last read position
            }

            // Create a logger for periodic forwarding to Seq and LogBee
            var periodicLogger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_levelSwitch) // Set minimum logging level based on the level switch
                .Enrich.FromLogContext() // Enrich log events with context information
                .WriteTo.Seq(
                    serverUrl: _configuration["Seq:SeqServerUrl"]!, // Seq server URL
                    apiKey: _configuration["Seq:SeqApiKey"]!, // Seq API key
                    controlLevelSwitch: _levelSwitch) // Use the same logging level switch
                .AddSeliseLogTracker(_configuration, _services) // Add custom LogBee tracker
                .CreateLogger(); // Create the periodic logger

            // Forward each log entry to Seq and LogBee
            foreach (var logEntry in logEntries)
            {
                periodicLogger.Information(logEntry); // Log each entry as information
            }

            periodicLogger.Dispose(); // Dispose the periodic logger
        }
        else
        {
            _logger.LogWarning("Log file not found: {LogFilePath}", logFilePath); // Log a warning if the log file is not found
        }
    }
}