using core.data.Model;
using core.data.Persistence;
using Microsoft.EntityFrameworkCore;
using ticket.api.Model;
using ticket.api.Service;

namespace ticket.api.Repository;

public class TicketRepository(PersistenceContext persistenceContext, DocumentService documentService)
{
    public async Task<TicketNewDto> AddTicket(TicketCreateDto ticketCreateDto, long userId)
    {
        var strategy = persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = new Ticket
                {
                    Title = ticketCreateDto.Title,
                    Description = ticketCreateDto.Description,
                    UserId = userId,
                };
                persistenceContext.Tickets.Add(ticket);
                await persistenceContext.SaveChangesAsync();
                await documentService.UploadAttachment(ticketCreateDto.attachment, ticket.Id);
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
        var ticket = await persistenceContext.Tickets
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
        var tickets = await persistenceContext.Tickets
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
        var strategy = persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = await persistenceContext.Tickets.FindAsync(id);
                if (ticket == null) return 0;
                ticket.Title = ticketUpdateDto.Title;
                ticket.Description = ticketUpdateDto.Description;
                await persistenceContext.SaveChangesAsync();
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
        var strategy = persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync<int>(async () =>
        {
            using var transaction = await persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var ticket = await persistenceContext.Tickets.FindAsync(id);
                if (ticket == null ) return 0;
                persistenceContext.Remove(ticket);
                await persistenceContext.SaveChangesAsync();
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
