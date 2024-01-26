using System.ComponentModel.DataAnnotations;
using core.utils.Validation;

namespace document.api.Model;

public record DocumentCreateDto([Required] long TicketId, [RequiredFile] IFormFile Attachment);