using System.Security.Claims;
using ticket.api.Model;
using ticket.api.Repository;

namespace ticket.api.Service;

public class TicketService
{
    private readonly TicketRepository _ticketRepository;
    private readonly DocumentService _documentService;
    private readonly ClaimsPrincipal _user;

    public TicketService(TicketRepository ticketRepository, DocumentService documentService, IHttpContextAccessor contextAccessor)
    {
        _ticketRepository = ticketRepository;
        _documentService = documentService;
        _user = contextAccessor.HttpContext.User;
    }

    public async Task<List<TicketItemDto>> GetTicketList()
    {
        var tickets = await _ticketRepository.SearchTickets();
        return tickets;
    }

    public async Task<TicketDetailsDto?> GetTicketDetails(long id)
    {
        var ticket = await _ticketRepository.FindTicket(id);
        return ticket;
    }

    public async Task<TicketNewDto> SaveTicket(TicketCreateDto ticketCreateDto)
    {
        var userId = long.Parse(_user.FindFirstValue(ClaimTypes.Name));
        var ticket = await _ticketRepository.AddTicket(ticketCreateDto, userId);
        return ticket;
    }

    public async Task<int> UpdateTicket(TicketUpdateDto ticketUpdateDto, long id)
    {
        var result = await _ticketRepository.UpdateTicket(ticketUpdateDto, id);
        return result;
    }

    public async Task<int> RemoveTicket(long id)
    {
        var result = await _ticketRepository.DeleteTicket(id);
        return result;
    }
}
