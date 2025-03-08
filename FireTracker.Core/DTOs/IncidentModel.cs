namespace FireTracker.Core.DTOs;

public class IncidentModel
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public RelativeLocation Location { get; set; }
    
    public FireDetectionResult Likelihood { get; set; }
}