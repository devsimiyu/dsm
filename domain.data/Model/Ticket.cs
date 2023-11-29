using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.data.Model;

[Table("tickets", Schema = "public")]
public class Ticket : EntityBase
{
    [Column("title")]
    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    [Column("description", TypeName = "text")]
    [Required]
    public string? Description { get; set; }

    [Column("user_id")]
    [Required]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    public List<Document> Documents { get; } = new List<Document>();
}
