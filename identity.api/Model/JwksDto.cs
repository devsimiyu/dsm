using System.Text.Json.Serialization;

namespace identity.api.Model;

public class JwksDto
{
    [JsonPropertyName("keys")]
    public List<object> Keys { get; set; } = new List<object>();
}
