using identity.api.Model;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Text.Json;

namespace identity.api.Service;

public class OpenIdConnectService
{
    private readonly IServer _server;
    private readonly IConfiguration _keys;

    public OpenIdConnectService(IServer server, IConfiguration configuration)
    {
        _server = server;
        _keys = configuration.GetSection("Certs");
    }

    public OpenIdConnectConfigDto GetConfigMetadata()
    {
        var host = _server.Features.Get<IServerAddressesFeature>().Addresses.First();
        var config = new OpenIdConnectConfigDto
        {
            JwksUri = host + "/.well-known/jwks.json",
            Issuer = host,
        };
        return config;
    }

    public async Task<JwksDto> GetSigningKeys()
    {
        var file = _keys["Jwks"];
        var json = await File.ReadAllTextAsync(file);
        var key = JsonSerializer.Deserialize<object>(json);
        var jwks = new JwksDto
        {
            Keys = new List<object> { key }
        };
        return jwks;
    }
}
