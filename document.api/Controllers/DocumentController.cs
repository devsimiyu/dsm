using System.Net.Mime;
using document.api.Model;
using document.api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace document.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocumentController(DocumentService documentService)
        => _documentService = documentService ?? throw new ArgumentNullException();

    [HttpGet("{id}", Name = "GetDocument")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentDetailsDto>> GetDocument([FromRoute] long id)
    {
        var documentDetailsDto = await _documentService.Get(id);
        return documentDetailsDto == null ? NotFound() : Ok(documentDetailsDto);
    }
    
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentNewDto>> SaveDocument([FromForm] DocumentCreateDto documentCreateDto)
    {
        var documentNewDto = await _documentService.Save(documentCreateDto);
        return CreatedAtAction("GetDocument", new { id = documentNewDto.Id }, documentNewDto);
    }
}
