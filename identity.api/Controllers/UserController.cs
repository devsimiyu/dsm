using identity.api.Model;
using identity.api.Service;
using Microsoft.AspNetCore.Mvc;

namespace identity.api.Controllers;

[Route("api")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
        => _userService = userService;

    [HttpPost("login")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<UserTokenDto>> Login([FromBody] UserLoginDto userLoginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var token = await _userService.AuthenticateUser(userLoginDto);
        var userToken = new UserTokenDto
        {
            AccessToken = token
        };
        return Ok(userToken);
    }
}
