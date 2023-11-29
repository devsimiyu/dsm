namespace ticket.api.Model;

public class TicketDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<TicketDocumentDto> Documents { get; set; } = new List<TicketDocumentDto>();
}
