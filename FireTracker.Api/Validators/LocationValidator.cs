using FireTracker.Api.DTOs;
using FluentValidation;

namespace FireTracker.Api.Validators;

public class LocationValidator : AbstractValidator<LocationRequest>
{
    public LocationValidator()
    {
        RuleFor(x => x.FireId).NotEmpty().WithMessage("Fire ID cannot be empty");
        RuleFor(x => x.Location).NotEmpty().WithMessage("Location cannot be empty");
    }
}