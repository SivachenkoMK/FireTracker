using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FireTracker.Utils;

public static class ControllerBaseExtensions
{
    public static async Task<IActionResult?> ErrorOrNull<T>(this ControllerBase controller, T request,
        ILogger logger, AbstractValidator<T> validator, CancellationToken token)
    {
        var result = await validator.ValidateAsync(request, token);

        if (result.IsValid) return null;
        
        if (logger.IsEnabled(LogLevel.Debug))
        {
            var message = string.Join(", ", result.Errors.SelectMany(v => v.ErrorMessage));
            logger.LogDebug(message);
        }

        return controller.BadRequest(result.Errors);
    }
}