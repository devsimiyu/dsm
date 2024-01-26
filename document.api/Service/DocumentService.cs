using document.api.Model;
using document.api.Repository;

namespace document.api.Service;

public class DocumentService
{
    private readonly DocumentRepository _documentRepository;

    public DocumentService(DocumentRepository documentRepository)
        => _documentRepository = documentRepository ?? throw new ArgumentNullException();
    
    public async Task<DocumentDetailsDto?> Get(long id)
    {
        return await _documentRepository.Find(id);
    }

    public async Task<DocumentNewDto> Save(DocumentCreateDto documentCreateDto)
    { 
        return await _documentRepository.Save(documentCreateDto);
    }
}
