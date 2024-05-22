using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.LogBee;
using Serilog.Sinks.LogBee.AspNetCore;

namespace LogTracker;

public static class SerilogConfigurationExtensions
{
    public static LoggerConfiguration AddSeliseLogTracker(this LoggerConfiguration loggerConfiguration, IConfiguration configuration, IServiceProvider services)
    {
        var logBeeConfig = new Configuration
        {
            OrganizationId = configuration["LogTracker:OrganizationId"]!,
            ApplicationId = configuration["LogTracker:ApplicationId"]!,
            ApiUrl = configuration["LogTracker:ApiUrl"]!
        };

        // Perform validation or additional configuration as needed

        return loggerConfiguration.WriteTo.LogBee(new LogBeeApiKey(logBeeConfig.OrganizationId, logBeeConfig.ApplicationId, logBeeConfig.ApiUrl),services);
    }

    public static IApplicationBuilder UseSeliseLogTrackerMiddleware(this IApplicationBuilder app)
    {
        // Add LogBee middleware here
        return app.UseLogBeeMiddleware();
    }
}
