using System.ComponentModel.DataAnnotations;

namespace ticket.api.Model;

public class TicketCreateDto
{
    [Required]
    public string Title { get; set; }

    public string Description { get; set; }
}
