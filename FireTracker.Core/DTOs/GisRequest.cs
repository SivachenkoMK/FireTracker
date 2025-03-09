namespace FireTracker.Core.DTOs;

public class GisRequest
{
    public Guid SessionGuid { get; set; }
    
    public double Longitude { get; set; }
    
    public double Latitude { get; set; }
}