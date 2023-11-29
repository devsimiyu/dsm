using System.ComponentModel.DataAnnotations;

namespace ticket.api.Model;

public class TicketUpdateDto
{
    [Required(AllowEmptyStrings = false)]
    public string Title { get; set; }

    public string? Description { get; set; }
}
