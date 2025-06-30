using System.Text.Json.Serialization;

namespace FilmsDB.Domain.Dto;

public class PosterData
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
