using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace core.utils.Validation;

public class RequiredFileAttribute : ValidationAttribute
{
    public RequiredFileAttribute()
        => ErrorMessage ??= "File is invalid";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file == null) return new ValidationResult(
            "File is required", new []{ validationContext.DisplayName });
        var filesAllowed = new []{ ".pdf", ".txt", ".docx" };
        var fileExtension = Path.GetExtension(file.FileName);
        return filesAllowed.Contains(fileExtension)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}