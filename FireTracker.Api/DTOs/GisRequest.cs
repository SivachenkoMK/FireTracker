namespace FireTracker.Api.DTOs;

public class GisRequest
{
    public string Gis { get; set; } = default!;
    
    public Guid FireId { get; set; }
}