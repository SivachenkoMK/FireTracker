namespace FireTracker.Core.Options;

public class InboundQueueConfiguration
{
    public string IncidentQueue { get; set; } = default!;
    
    public string AnalysisQueue { get; set; } = default!;
}