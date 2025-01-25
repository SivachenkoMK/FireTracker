using FireTracker.Api.DTOs;
using FluentValidation;

namespace FireTracker.Api.Validators;

public class GisValidator : AbstractValidator<GisRequest>
{
    public GisValidator()
    {
        RuleFor(request => request.Gis).NotEmpty(); // Will we adjusted to fit the incoming details
    }
}