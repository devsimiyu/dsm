using identity.api.Model;
using identity.api.Repository;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace identity.api.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IConfiguration _keys;
    private readonly IServer _server;

    public UserService(UserRepository userRepository, IConfiguration configuration, IServer server)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException();
        _keys = configuration.GetSection("Certs");
        _server = server;
    }

    public async Task<string> AuthenticateUser(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.FindUserByEmail(userLoginDto.Email);
        if (user == null) throw new Exception("User not found");
        var securityRsa = await GetRsaKey();
        var securityKey = new RsaSecurityKey(securityRsa);
        var securityCreds = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        var securityJwt = new JwtSecurityToken(
            new JwtHeader(securityCreds),
            new JwtPayload(
                issuer: _server.Features.Get<IServerAddressesFeature>().Addresses.First(),
                audience: "ticket",
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(2),
                claims: new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                }));
        var token = new JwtSecurityTokenHandler().WriteToken(securityJwt);
        return token;
    }

    private async Task<RSA> GetRsaKey()
    {
        var privateRsaXmlKey = await File.ReadAllTextAsync(_keys["Private"]);
        var rsa = RSA.Create();
        rsa.FromXmlString(privateRsaXmlKey);
        return rsa;
    }
}
