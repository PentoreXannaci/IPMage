using Microsoft.Extensions.Logging;

namespace IPMage.DnsServices;

public abstract class ServiceBase(ILogger logger)
{
  public ILogger Logger = logger;
  public string CurrentIp { get; set; } = string.Empty;
  public bool Initialized { get; set; }

  public abstract Task UpdateIPAsync(string ip);
}