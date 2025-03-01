namespace FireTracker.Core.DTOs;

public class IncidentModel
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public string Location { get; set; } = default!;
    
    public FireDetectionResult Likelihood { get; set; }
}