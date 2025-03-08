using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace FireTracker.Core.DTOs;

[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RelativeLocation
{
    Unknown,
    Inside,
    Outside
}