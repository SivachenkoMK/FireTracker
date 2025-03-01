namespace FireTracker.Analysis.DTOs;

public class AnalysisResult
{
    public Guid SessionId { get; set; }
    
    public FireDetectionResult Detection { get; set; }
}