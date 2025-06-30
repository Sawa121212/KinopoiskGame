using FilmsDB.Domain.Models;

namespace FilmsDB.Infrastructure.Interfaces.Services;

public interface IFilmImportService
{
    Task<Category?> ImportFromJsonAsync(string? filePath);
}
