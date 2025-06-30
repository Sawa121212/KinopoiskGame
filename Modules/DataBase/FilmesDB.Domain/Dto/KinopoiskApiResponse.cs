namespace FilmsDB.Domain.Dto;

public class KinopoiskApiResponse
{
    public List<FilmData> Docs { get; set; }

    public List<FilmData> Items { get; set; }

    public List<FilmData> Films => Docs ?? Items;
}
