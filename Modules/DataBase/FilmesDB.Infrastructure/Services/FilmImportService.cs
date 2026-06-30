using System.Text.Json;
using System.Xml.Linq;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Services;

namespace FilmsDB.Infrastructure.Services;

public class FilmImportService : IFilmImportService
{
    public async Task<Category?> ImportFromJsonAsync(string? filePath)
    {
        string? fileName = Path.GetFileNameWithoutExtension(filePath);

        if (fileName == null)
        {
            return null;
        }
        else
        {
            string categoryName = fileName.Split('_')[0]; // "top250", "popular-films" и т.д.

            //Category? category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            Category? category = new()
            {
                Name = categoryName
            };

            string json = await File.ReadAllTextAsync(filePath);

            // Динамический анализ структуры JSON
            using JsonDocument jsonDoc = JsonDocument.Parse(json);
            JsonElement root = jsonDoc.RootElement;

            List<Film?> films = new();

            // Определяем тип JSON структуры
            switch (root.ValueKind)
            {
                case JsonValueKind.Object when root.TryGetProperty("docs", out JsonElement docsElement):
                    // Формат: { "docs": [...] }
                    films = ParseFilmsFromArray(docsElement, category.Id);

                    break;

                case JsonValueKind.Object when root.TryGetProperty("items", out JsonElement itemsElement):
                    // Формат: { "items": [...] }
                    films = ParseFilmsFromArray(itemsElement, category.Id);

                    break;

                case JsonValueKind.Array:
                    // Формат: [...]
                    films = ParseFilmsFromArray(root, category.Id);

                    break;

                default:
                    throw new InvalidOperationException("Неизвестный формат JSON");
            }

            category.Films.AddRange(films);

            return category;
        }
    }

    private List<Film?> ParseFilmsFromArray(JsonElement arrayElement, int categoryId)
    {
        List<Film?> films = new();

        foreach (JsonElement filmElement in arrayElement.EnumerateArray())
        {
            try
            {
                films.Add(new Film
                {
                    Name = GetStringProperty(filmElement, "name")
                        ?? GetStringProperty(filmElement, "alternativeName")
                        ?? "Без названия",
                    Rating = GetNestedDoubleProperty(filmElement, "rating", "kp"),
                    PosterUrl = GetNestedStringProperty(filmElement, "poster", "url"),
                    Year = GetNestedIntegerProperty(filmElement, "year"),
                    UId = GetNestedIntegerProperty(filmElement, "id"),
                    CategoryId = categoryId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки фильма: {ex.Message}");
            }
        }

        return films;
    }

    // Вспомогательные методы для безопасного доступа к свойствам
    private string GetStringProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement prop)
            ? prop.GetString()
            : null;
    }

    private int GetNestedIntegerProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
        {
            return prop.ValueKind == JsonValueKind.Number
                ? prop.GetInt32()
                : -1;
        }

        return -1;
    }

    private double? GetNestedDoubleProperty(JsonElement element, string outerProp, string innerProp)
    {
        if (element.TryGetProperty(outerProp, out JsonElement outer)
            && outer.TryGetProperty(innerProp, out JsonElement inner))
        {
            return inner.ValueKind == JsonValueKind.Number
                ? inner.GetDouble()
                : (double?) null;
        }

        return null;
    }

    private string GetNestedStringProperty(JsonElement element, string outerProp, string innerProp)
    {
        if (element.TryGetProperty(outerProp, out JsonElement outer) && outer.TryGetProperty(innerProp, out JsonElement inner))
        {
            return inner.GetString();
        }

        return null;
    }
}