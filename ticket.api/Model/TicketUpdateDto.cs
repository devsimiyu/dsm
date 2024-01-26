using System.ComponentModel.DataAnnotations;

namespace ticket.api.Model;

public class TicketCreateDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public IFormFile attachment { get; set; }
}
