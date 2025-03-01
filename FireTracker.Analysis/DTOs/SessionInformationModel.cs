namespace FireTracker.Analysis.DTOs;

public class SessionInformationModel
{
    public Guid SessionGuid { get; set; }

    public string PhotoUrl { get; set; } = default!;
    
    public int PhotoLength { get; set; }
}