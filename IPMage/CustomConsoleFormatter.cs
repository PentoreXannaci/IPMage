using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace IPMage;

public class CustomConsoleFormatter() : ConsoleFormatter("custom"), IDisposable
{
  public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
  {
    var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
    if (message is null) return;

    // Extract just the class name from the category name
    var className = logEntry.Category[(logEntry.Category.LastIndexOf('.') + 1)..];

    var color = logEntry.LogLevel switch
    {
      LogLevel.Trace => "\x1b[90m", // gray
      LogLevel.Debug => "\x1b[34m", // blue
      LogLevel.Information => "\x1b[32m", // green
      LogLevel.Warning => "\x1b[33m", // Yellow
      LogLevel.Error => "\x1b[31m", // Red
      LogLevel.Critical => "\x1b[41m", // Red background
      _ => "\x1b[0m" // Reset
    };

    var resetColor = "\x1b[0m";

    // Write the log message in the desired format
    textWriter.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] [{color}{logEntry.LogLevel.ToString().ToLower()}{resetColor}] {className} {message}");
  }

  public void Dispose() { }
}