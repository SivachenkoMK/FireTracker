using System.Collections.Concurrent;
using FireTracker.Core.DTOs;
using FireTracker.Core.Persistence;

namespace FireTracker.Core.Services;

public class IncidentService
{
    private readonly ILogger<IncidentService> _logger;

    public IncidentService(ILogger<IncidentService> logger)
    {
        _logger = logger;
    }

    public void UpdateLocation(LocationRequest model)
    {
        if (DbContext.IncidentCollection.TryGetValue(model.FireId, out var incident))
        {
            incident.Location = model.Location;
            incident.UpdatedAtUtc = DateTime.UtcNow;
        }
        
        var incidentModel = new IncidentModel
        {
            Id = model.FireId,
            Location = model.Location,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        
        if (!DbContext.IncidentCollection.TryAdd(model.FireId, incidentModel))
            _logger.LogError("Failed to add incident with id {Id} to the collection", model.FireId);
    }

    public void UpdateAnalysis(AnalysisResult analysisResult)
    {
        if (DbContext.IncidentCollection.TryGetValue(analysisResult.SessionId, out var incident))
        {
            incident.Likelihood = analysisResult.Detection;
            incident.UpdatedAtUtc = DateTime.UtcNow;
        }
        
        var incidentModel = new IncidentModel
        {
            Id = analysisResult.SessionId,
            Likelihood = analysisResult.Detection,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        
        if (!DbContext.IncidentCollection.TryAdd(analysisResult.SessionId, incidentModel))
            _logger.LogError("Failed to add incident with id {Id} to the collection", analysisResult.SessionId);
    }
}