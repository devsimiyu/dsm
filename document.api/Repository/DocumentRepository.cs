using document.api.Model;
using core.data.Model;
using core.data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace document.api.Repository;

public class DocumentRepository
{
    private readonly PersistenceContext _persistenceContext;

    public DocumentRepository(PersistenceContext persistenceContext)
        => _persistenceContext = persistenceContext ?? throw new ArgumentNullException();

    public async Task<DocumentDetailsDto?> Find(long id)
    {
        var document = await _persistenceContext.Documents.FindAsync(id);
        if (document == null) return null;
        var documentDetailsDto = new DocumentDetailsDto(
            document.Id, document.Name, document.Path, "", document.TicketId);
        return documentDetailsDto;
    }
    
    public async Task<DocumentNewDto> Save(DocumentCreateDto documentCreateDto)
    {
        var strategy = _persistenceContext.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _persistenceContext.Database.BeginTransactionAsync();
            try
            {
                var document = new Document
                {
                    TicketId = documentCreateDto.TicketId,
                    Name = documentCreateDto.Attachment.FileName,
                    Path = $"documents/{Guid.NewGuid()}"
                };
                _persistenceContext.Documents.Add(document);
                await _persistenceContext.SaveChangesAsync();
                var documentNewDto = new DocumentNewDto(document.Id, document.Name, document.Path, document.TicketId);
                await transaction.CommitAsync();
                return documentNewDto;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
        return result;
    }
}