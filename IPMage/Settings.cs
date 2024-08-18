
using IPMage.Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IPMage;

public class Settings
{
  public static readonly string UserAgent = $"IPMage/{Environment.Version.ToString(3)} ({Environment.OSVersion.Platform}; {Environment.Is64BitOperatingSystem}) (https://github.com/PentoreXannaci/IPMage)";
  public static string LogLevel = "information";

  public static List<ServiceSettingsBase> Services { get; set; } = [];

  public static void Load()
  {
    var path = Path.Combine(AppContext.BaseDirectory, "settings.json");
    if (!File.Exists(path))
    {
      Console.WriteLine("SETTINGS FILE MISSING.");
      Environment.Exit(1);
    }

    var json = File.ReadAllText(path);
    var settings = JsonConvert.DeserializeObject<JObject>(json);

    // parse log level
    LogLevel = settings?["logLevel"]?.ToString() ?? "info";

    // parse service settings
    var services = settings?["services"];
    if(services == null)
    {
      Console.WriteLine("NO SERVICES CONFIGURED.");
      Environment.Exit(1);
    }

    foreach (var service in (services as JArray)!)
    {
      var serviceType = (string)service["serviceType"]!;
      if(string.IsNullOrEmpty(serviceType))
      {
        Console.WriteLine("SERVICE TYPE MISSING.");
        continue;
      }

      ServiceSettingsBase? serviceSettings = serviceType switch
      {
        "cloudflare" => service.ToObject<CloudflareSettings>(),
        _ => null
      };

      if (serviceSettings == null)
      {
        Console.WriteLine("INVALID SERVICE SETTINGS.\n" + service);
        Environment.Exit(1);
      }

      Services.Add(serviceSettings);
    }

  }
}