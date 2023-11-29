namespace ticket.api.Model;

public class TicketItemDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public TicketUserDto User { get; set; }
}