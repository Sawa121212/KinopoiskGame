using FilmsDB.Domain.Models;
using Infrastructure.Domain.Helpers;
using Common.Extensions.Collections;

namespace Game.Domain.Models;

public class GameMode
{
    public GameMode(string name, List<Film> films)
    {
        Name = name;
        _allFilms = films;
    }

    public string Name { get; }

    public int FilmsCount => _allFilms.Count();

    public IEnumerable<Film> GameFilms { get; private set; }

    public int GameFilmsCount => GameFilms.Count();

    public bool CreateGameFilms(int count)
    {
        if (count == FilmsCount)
        {
            GameFilms = _allFilms.Shuffle();
            return true;
        }

        List<Film> currentFilms = new();

        while (count > currentFilms.Count)
        {
            int index = RandomGenerator.GetRandom().Next(0, FilmsCount);

            Film film = _allFilms[index];

            if (currentFilms.Contains(film))
            {
                continue;
            }

            currentFilms.Add(film);
        }

        GameFilms = currentFilms;

        return true;
    }

    private readonly List<Film> _allFilms;
}