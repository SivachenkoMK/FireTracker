using Microsoft.AspNetCore.Mvc;

namespace FireTracker.Api.DTOs;

public class PhotoRequest
{
    [FromForm]
    public IFormFile Photo { get; set; } = default!;
    
    public Guid SessionGuid { get; set; }
}