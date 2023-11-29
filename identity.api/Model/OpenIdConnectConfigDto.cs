using System.Text.Json.Serialization;

namespace identity.api.Model;

public class OpenIdConnectConfigDto
{
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; }

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }
}
