using FireTracker.Analysis.DTOs;

namespace FireTracker.Analysis.Services;

public class InterpretationService
{
    public FireDetectionResult InterpretAnalysedResult(float processingResult)
    {
        return processingResult switch
        {
            < 0.3f => FireDetectionResult.Fire,
            >= 0.3f and < 0.5f => FireDetectionResult.LikelyFire,
            >= 0.5f and < 0.85f => FireDetectionResult.LikelyNoFire,
            >= 0.85f => FireDetectionResult.NoFire,
            _ => FireDetectionResult.Unknown
        };
    }
}