using FireTracker.Api.DTOs;
using FluentValidation;

namespace FireTracker.Api.Validators;

public class GisValidator : AbstractValidator<GisRequest>
{
    public GisValidator()
    {
        RuleFor(request => request.SessionGuid).NotEmpty();
        RuleFor(model => model.Latitude).LessThanOrEqualTo(80)
            .GreaterThanOrEqualTo(-80)
            .WithMessage("Latitude must be between -80 <-> 80 degress.");

        RuleFor(model => model.Longitude).LessThanOrEqualTo(180)
            .GreaterThanOrEqualTo(-180)
            .WithMessage("Longitude must be between -180 <-> 180 degress.");    }
}