using domain.data.Model;
using domain.data.Persistence;
using Microsoft.EntityFrameworkCore;
using ticket.api.Model;

namespace ticket.api.Repository;

public class TicketRepository
{
    private readonly PersistenceContext _persistenceContext;

    public TicketRepository(PersistenceContext persistenceContext)
        => _persistenceContext = persistenceContext;

    public async Task<TicketNewDto> AddTicket(TicketCreateDto ticketCreateDto, long userId)
    {
        var strategy = _persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = new Ticket
                {
                    Title = ticketCreateDto.Title,
                    Description = ticketCreateDto.Description,
                    UserId = userId,
                };
                _persistenceContext.Tickets.Add(ticket);
                await _persistenceContext.SaveChangesAsync();
                var ticketNewDto = new TicketNewDto
                {
                    Id = ticket.Id,
                    Title = ticketCreateDto.Title,
                    Description = ticketCreateDto.Description,
                };
                await transaction.CommitAsync();
                return ticketNewDto;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
        return result;
    }

    public async Task<TicketDetailsDto?> FindTicket(long id)
    {
        var ticket = await _persistenceContext.Tickets
            .Include(ticket => ticket.Documents.Where(document => document.DeletedAt == null))
            .Where(ticket => ticket.Id == id)
            .FirstOrDefaultAsync();
        if (ticket == null) return null;
        var ticketDetailsDto = new TicketDetailsDto
        {
            Title = ticket.Title,
            Description = ticket.Description,
            Documents = ticket.Documents.Select(document => new TicketDocumentDto
            {
                Id = document.Id,
                Name = document.Name,
            })
            .ToList(),
        };
        return ticketDetailsDto;
    }

    public async Task<List<TicketItemDto>> SearchTickets()
    {
        var tickets = await _persistenceContext.Tickets
            .Include(ticket => ticket.User)
            .Where(ticket => ticket.DeletedAt == null)
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToListAsync();
        var ticketItemDto = tickets.Select(ticket => new TicketItemDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            User = new TicketUserDto
            {
                Id = ticket.User.Id,
                FirstName = ticket.User.FirstName,
                LastName = ticket.User.LastName,
                Email = ticket.User.Email,
            }
        })
        .ToList();
        return ticketItemDto;
    }

    public async Task<int> UpdateTicket(TicketUpdateDto ticketUpdateDto, long id)
    {
        var strategy = _persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = await _persistenceContext.Tickets.FindAsync(id);
                if (ticket == null) return 0;
                ticket.Title = ticketUpdateDto.Title;
                ticket.Description = ticketUpdateDto.Description;
                await _persistenceContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return 1;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
        return result;
    }

    public async Task<int> DeleteTicket(long id)
    {
        var strategy = _persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync<int>(async () =>
        {
            using var transaction = await _persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = await _persistenceContext.Tickets.FindAsync(id);
                if (ticket == null ) return 0;
                _persistenceContext.Remove(ticket);
                await _persistenceContext.SaveChangesAsync();
                return 1;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
        return result;
    }
}
