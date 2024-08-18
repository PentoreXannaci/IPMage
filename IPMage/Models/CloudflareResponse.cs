using Newtonsoft.Json;

namespace IPMage.Models;

public class CloudflareResponse
{
  public List<Error> Errors { get; set; } = [];

  public List<Message> Messages { get; set; } = [];

  public bool Success { get; set; }

  public DnsRecord? Result { get; set; }
}

public class DnsRecord
{
  public string Content { get; set; } = string.Empty;

  public string Name { get; set; } = string.Empty;

  public bool Proxied { get; set; } = false;

  public string Type { get; set; } = string.Empty;

  public string Comment { get; set; } = string.Empty;

  [JsonProperty("created_on")]
  public DateTime CreatedOn { get; set; }

  public string Id { get; set; } = string.Empty;

  public Meta? Meta { get; set; }

  [JsonProperty("modified_on")]
  public DateTime ModifiedOn { get; set; }

  public bool Proxiable { get; set; }

  public List<string> Tags { get; set; } = [];

  public int Ttl { get; set; }
}

public class Meta
{
  [JsonProperty("auto_added")]
  public bool AutoAdded { get; set; }

  public string Source { get; set; } = string.Empty;
}

public class Error
{
  public int Code { get; set; }

  public string Message { get; set; } = string.Empty;
}

public class Message
{
  public int Code { get; set; }

  [JsonProperty("message")]
  public string MessageContent { get; set; } = string.Empty;
}