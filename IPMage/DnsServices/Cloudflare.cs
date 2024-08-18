using System.Text;
using IPMage.Models;
using IPMage.Models.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IPMage.DnsServices;

public class Cloudflare : ServiceBase
{
  private readonly CloudflareSettings _settings;
  private readonly HttpClient _client;

  public Cloudflare(ILogger logger, CloudflareSettings settings) : base(logger)
  {
    _settings = settings;

    _client = new HttpClient();
    _client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);
    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _settings.ApiKey);
    _client.BaseAddress = new Uri("https://api.cloudflare.com");

    LoadIp();
  }

  private async void LoadIp()
  {
    Logger.LogInformation("({type}, {name}) Loading current IP...", _settings.ServiceType, _settings.Name);

    while (!Initialized)
    {
      try
      {
        var response = await _client.GetAsync($"/client/v4/zones/{_settings.ZoneId}/dns_records/{_settings.RecordId}");
        if (!response.IsSuccessStatusCode)
        {
          Logger.LogError("({type}, {name}) Failed to fetch current IP", _settings.ServiceType, _settings.Name);
          await LogAndWaitRetry();
          continue;
        }

        var content = await response.Content.ReadAsStringAsync();
        var cloudflareResponse = JsonConvert.DeserializeObject<CloudflareResponse>(content);
        if (cloudflareResponse == null)
        {
          Logger.LogError("({type}, {name}) Failed to deserialize response", _settings.ServiceType, _settings.Name);
          await LogAndWaitRetry();
          continue;
        }

        if (!cloudflareResponse.Success)
        {
          Logger.LogError("({type}, {name}) Failed to fetch IP.\nResponse: {resp}", _settings.ServiceType, _settings.Name, content);
          await LogAndWaitRetry();
          continue;
        }

        if (string.IsNullOrEmpty(cloudflareResponse.Result?.Content))
        {
          Logger.LogError("({type}, {name}) Failed to fetch IP.\nResponse: {resp}", _settings.ServiceType, _settings.Name, content);
          await LogAndWaitRetry();
          continue;
        }

        CurrentIp = cloudflareResponse.Result.Content;
        Initialized = true;
      }
      catch (Exception e)
      {
        Logger.LogError(e, "({type}, {name}) Exception fetching IP", _settings.ServiceType, _settings.Name);
        await LogAndWaitRetry();
      }
    }
  }

  private async Task LogAndWaitRetry()
  {
    Logger.LogInformation("({type}, {name}) Retrying in 5 seconds...", _settings.ServiceType, _settings.Name);
    await Task.Delay(5000);
  }

  public override async Task UpdateIPAsync(string ip)
  {
    // ip not loaded yet.
    if (string.IsNullOrEmpty(CurrentIp))
      return;

    try
    {
      var content = new StringContent(JsonConvert.SerializeObject(new
      {
        content = ip
      }), Encoding.UTF8, "application/json");

      var response = await _client.PatchAsync($"/client/v4/zones/{_settings.ZoneId}/dns_records/{_settings.RecordId}", content);
      if(!response.IsSuccessStatusCode)
      {
        Logger.LogError("({type}, {name}) Failed to update IP", _settings.ServiceType, _settings.Name);
        return;
      }

      CurrentIp = ip;
      Logger.LogInformation("({type}, {name}) IP({ip}) updated successfully", _settings.ServiceType, _settings.Name, CurrentIp);
    }
    catch (Exception e)
    {
      Logger.LogError(e, "({type}, {name}) Exception updating IP", _settings.ServiceType, _settings.Name);
      throw;
    }
  }
}