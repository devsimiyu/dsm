using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.data.Model;

[Table("documents", Schema = "public")]
public class Document : EntityBase
{
    [Column("name")]
    [Required]
    public string Name { get; set; }

    [Column("path")]
    [Required]
    public string Path { get; set; }

    [Column("ticket_id")]
    [Required]
    public long TicketId { get; set; }

    [ForeignKey(nameof(TicketId))]
    public Ticket Ticket { get; set; }
}
