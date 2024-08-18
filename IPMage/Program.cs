using IPMage;
using IPMage.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

Settings.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
switch (Settings.LogLevel)
{
  case "information":
    builder.Logging.SetMinimumLevel(LogLevel.Information);
    break;
  case "debug":
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
    break;
  case "trace":
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    break;
  case "warn":
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    break;
  case "error":
    builder.Logging.SetMinimumLevel(LogLevel.Error);
    break;
  default:
    builder.Logging.SetMinimumLevel(LogLevel.Information);
    break;
}

builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, SimpleConsoleFormatterOptions>();
builder.Logging.AddConsole(options =>
{
  options.FormatterName = "custom";
});

builder.Services.AddHostedService<DnsUpdateService>();

var host = builder.Build();
await host.RunAsync();