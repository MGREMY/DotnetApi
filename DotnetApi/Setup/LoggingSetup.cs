using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace DotnetApi.Setup;

public static class LoggingSetup
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        if (bool.TryParse(builder.Configuration["OpenTelemetry:Enabled"], out var isEnabled))
        {
            if (isEnabled)
            {
                loggerConfiguration
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = builder.Configuration["OpenTelemetry:Endpoint"] ?? string.Empty;
                        options.Protocol = OtlpProtocol.HttpProtobuf;
                        options.Headers = new Dictionary<string, string>
                        {
                            ["X-Seq-ApiKey"] = builder.Configuration["OpenTelemetry:ApiKey"] ?? string.Empty,
                        };
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = builder.Environment.ApplicationName,
                            ["deployment.environment"] = builder.Environment.EnvironmentName
                        };
                    });
            }
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Services.AddSerilog();

        return builder;
    }
}