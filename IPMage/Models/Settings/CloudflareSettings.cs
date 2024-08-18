namespace IPMage.Models.Settings;

public class CloudflareSettings : ServiceSettingsBase
{
  public string ApiKey { get; set; } = string.Empty;
  public string ZoneId { get; set; } = string.Empty;
  public string RecordId { get; set; } = string.Empty;
}