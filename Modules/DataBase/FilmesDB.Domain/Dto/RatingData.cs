using System.Text.Json.Serialization;

namespace FilmsDB.Domain.Dto;

public class RatingData
{
    [JsonPropertyName("kp")]
    public double? Kp { get; set; }
}
