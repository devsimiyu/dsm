using identity.api.Service;
using Microsoft.AspNetCore.Mvc;

namespace identity.api.Controllers;

[Route(".well-known")]
[ApiController]
public class OpenIdConnectController : ControllerBase
{
    private readonly OpenIdConnectService _openIdConnectService;

    public OpenIdConnectController(OpenIdConnectService openIdConnectService)
        => _openIdConnectService = openIdConnectService;

    [HttpGet("openid-configuration")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetConfigurationMetadata()
    {
        var config = _openIdConnectService.GetConfigMetadata();
        return Ok(config);
    }

    [HttpGet("jwks.json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJsonWebKeySets()
    {
        var jwks = await _openIdConnectService.GetSigningKeys();
        return Ok(jwks);
    }
}
