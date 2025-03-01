namespace FireTracker.Core.DTOs;

public class AnalysisResult
{
    public Guid SessionId { get; set; }
    
    public FireDetectionResult Detection { get; set; }
}