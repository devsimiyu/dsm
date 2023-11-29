using System.ComponentModel.DataAnnotations;

namespace identity.api.Model;

public class UserLoginDto
{
    [Required]
    public string Email { get; set; }
    public string Password { get; set; }
}
