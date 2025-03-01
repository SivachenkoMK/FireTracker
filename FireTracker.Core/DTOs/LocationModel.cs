namespace FireTracker.Core.DTOs;

public class LocationModel
{
    public string Location { get; set; } = default!;
    
    public Guid FireId { get; set; }
}