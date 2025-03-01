using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace FireTracker.Analysis.DTOs;

[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FireDetectionResult
{
    Unknown,
    NoFire,
    LikelyNoFire,
    LikelyFire,
    Fire
}