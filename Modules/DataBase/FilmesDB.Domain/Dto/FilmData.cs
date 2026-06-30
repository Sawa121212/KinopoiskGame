using System.Text.Json.Serialization;

namespace FilmsDB.Domain.Dto;

public class FilmData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("alternativeName")]
    public string AlternativeName { get; set; }

    [JsonPropertyName("rating")]
    public RatingData Rating { get; set; }

    [JsonPropertyName("poster")]
    public PosterData Poster { get; set; }
}