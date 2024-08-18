using System.Net;
using IPMage.DnsServices;
using IPMage.Models.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IPMage.Services;

public class DnsUpdateService : IHostedService, IDisposable
{
  private readonly ILogger<DnsUpdateService> _logger;

  private Timer? _timer;
  private readonly HttpClient _httpClient;
  private readonly string[] _ipSites =
  [
    "http://icanhazip.com",
    "http://ifconfig.me/ip",
    "http://myexternalip.com/raw"
  ];

  private readonly List<ServiceBase> _services = [];

  public DnsUpdateService(ILogger<DnsUpdateService> logger)
  {
    _logger = logger;
    _httpClient = new HttpClient();
    _httpClient.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogDebug("IPFetchService is Starting.");
    _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

    InitializeServices();
    
    return Task.CompletedTask;
  }

  private void InitializeServices()
  {
    var services = Settings.Services;

    foreach (var service in services)
    {
      if (service is CloudflareSettings cloudflareSettings)
      {
        _services.Add(new Cloudflare(_logger, cloudflareSettings));
      }
    }
  }

  private async void DoWork(object? state)
  {
    _logger.LogDebug("Fetching IP...");
    var ip = string.Empty;
    foreach (var site in _ipSites)
    {
      try
      {
        ip = await _httpClient.GetStringAsync(site);
        ip = ip.Trim();
        if (!IPAddress.TryParse(ip, out _))
        {
          _logger.LogWarning("IP address is invalid ipv4\n{ip}", ip);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching IP");
      }
    }
    
    if (string.IsNullOrEmpty(ip))
    {
      _logger.LogError("Failed to fetch IP, string is NULL or EMPTY");
      return;
    }

    _logger.LogDebug("Fetched IP: " + ip);

    // update all services with different IP.
    foreach (var service in _services.Where(service => service.CurrentIp != ip && service.Initialized))
    {
      _logger.LogInformation("Updating IP for Service: {service}", service.GetType().Name);
      await service.UpdateIPAsync(ip);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogDebug("IPFetchService is stopping.");
    _timer?.Change(Timeout.Infinite, 0);
    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _timer?.Dispose();
  }
}