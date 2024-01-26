namespace ticket.api.Service;

public class DocumentService(HttpClient http)
{
    public async Task UploadAttachment(IFormFile attachment, long ticketId)
    {
        await using var memoryStream = new MemoryStream();
        await attachment.CopyToAsync(memoryStream);
        var formData = new MultipartFormDataContent
        {
            { new ByteArrayContent(memoryStream.ToArray()), "attachment", attachment.FileName },
            { new StringContent(ticketId.ToString()), "ticketId" }
        };
        var response = await http.PostAsync("/", formData);
        response.EnsureSuccessStatusCode();
    }
}
