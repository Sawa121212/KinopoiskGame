using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Services;
using Game.Domain.Models;
using Game.Infrastructure.Interfaces.Services;

namespace Game.Infrastructure.Services;

public class GameServices : IGameServices
{
    public GameServices(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IEnumerable<GameMode> CreateGameModes()
    {
        List<GameMode> gameModes = [];
        List<Film?> allFilms = [];

        List<Category> categories = _categoryService.GetAllCategories();

        if (categories.Count == 0)
        {
            return [];
        }

        foreach (Category category in categories)
        {
            gameModes.Add(new GameMode(category.Name, category.Films));
            allFilms.AddRange(category.Films);
        }

        gameModes.Insert(0, new GameMode("Все категории", allFilms));

        return gameModes;
    }

    public IEnumerable<GameMode> CreateGameModes2()
    {
        List<GameMode> gameModes = [];
        List<Film?> allFilms = [];

        List<Category> categories = _categoryService.GetAllCategories();

        if (categories.Count == 0)
        {
            return [];
        }

        foreach (Category category in categories)
        {
            gameModes.Add(new GameMode(category.Name, category.Films));
            allFilms.AddRange(category.Films);
        }

        gameModes.Insert(0, new GameMode("Все категории", allFilms));

        return gameModes;
    }

    private readonly ICategoryService _categoryService;
}