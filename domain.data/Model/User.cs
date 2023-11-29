using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.data.Model;

[Table("users", Schema = "public")]
public class User : EntityBase
{
    [Column("first_name")]
    [Required]
    [MaxLength(250)]
    public string FirstName { get; set; }

    [Column("last_name")]
    [Required]
    [MaxLength(250)]
    public string LastName { get; set; }

    [Column("email")]
    [Required]
    [MaxLength(250)]
    public string Email { get; set; }

    [Column("password")]
    [Required]
    [MaxLength(250)]
    public string Password { get; set; }

    [Column("role")]
    [Required]
    public UserRole Role { get; set; }

    public List<Ticket> Tickets { get; } = new List<Ticket>();
}

public enum UserRole
{
    ADMIN,
    MANAGER,
    STAFF,
}