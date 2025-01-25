namespace FireTracker.Api.DTOs;

public class LocationRequest
{
    public string Location { get; set; } = default!;
    
    public Guid FireId { get; set; }
}