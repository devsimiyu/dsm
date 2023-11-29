namespace ticket.api.Service;

public class DocumentService
{
    private readonly HttpClient _httpClient;

    public DocumentService(HttpClient httpClient)
        => _httpClient = httpClient;
}
