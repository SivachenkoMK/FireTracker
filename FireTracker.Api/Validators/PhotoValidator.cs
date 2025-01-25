using FireTracker.Api.DTOs;
using FluentValidation;

namespace FireTracker.Api.Validators;

public class PhotoValidator : AbstractValidator<PhotoRequest>
{
    public PhotoValidator()
    {
        RuleFor(request => request.Photo).Must(x => HasAllowedExtension(x.FileName)); // Will we adjusted to fit the incoming details
    }

    private bool HasAllowedExtension(string fileName)
    {
        var allowedExtensions = new[] { "jpg", "jpeg", "png" };
        var extension = fileName.Split('.')[^1];
        
        return allowedExtensions.Contains(extension);
    }
}