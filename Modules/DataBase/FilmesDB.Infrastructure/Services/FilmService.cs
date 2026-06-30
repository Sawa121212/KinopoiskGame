using FilmsDB.Domain;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Managers;
using FilmsDB.Infrastructure.Interfaces.Services;
using Infrastructure.Domain.Helpers;

namespace FilmsDB.Infrastructure.Services
{
    public class FilmService : IFilmService
    {
        public FilmService(IKinopoiskDbManager kinopoiskDbManager, ICategoryService topicService)
        {
            _kinopoiskDbManager = kinopoiskDbManager;
            _dbContext = _kinopoiskDbManager.DbContext;
            _topicService = topicService;
        }

        /// <inheritdoc />
        public async Task AddFilmsAsync(List<Film?> films)
        {
            bool shouldSave = false;

            foreach (Film? film in films.Where(film => GetFilmById(film.UId) == null))
            {
                _dbContext.Films.Add(film);
                shouldSave = true;
            }

            if (shouldSave)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public Film CreateFilm(Film film)
        {
            if (film is null)
            {
                throw new NullReferenceException($"[{nameof(CreateFilm)}] {nameof(film)} is null");
            }

            _dbContext.Films.Add(film);
            _dbContext.SaveChanges();

            return film;
        }

        /// <inheritdoc />
        public void UpdateFilm(Film updatedFilm)
        {
            if (updatedFilm is null)
            {
                throw new NullReferenceException($"[{nameof(UpdateFilm)}] {nameof(updatedFilm)} is null");
            }

            UpdateFilm(updatedFilm.Id, updatedFilm);
        }

        /// <inheritdoc />
        public void UpdateFilm(int filmId, Film updatedFilm)
        {
            Film? film = _dbContext.Films.Find(filmId);

            if (film == null)
            {
                return;
            }

            //
            // film.Text = updatedFilm.Text;
            // film.CorrectAnswer = updatedFilm.CorrectAnswer;
            // film.Price = updatedFilm.Price;
            // film.Picture = updatedFilm.Picture;
            // film.Music = updatedFilm.Music;

            // обновление других свойств
            _dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public void DeleteFilm(Film? film)
        {
            if (_dbContext.Films.Contains(film))
            {
                DeleteFilm(film.Id);
            }
        }

        /// <inheritdoc />
        public void DeleteFilm(int filmId)
        {
            Film? film = _dbContext.Films.Find(filmId);

            if (film == null)
            {
                return;
            }

            _dbContext.Films.Remove(film);
            _dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public Film? GetFilmById(int filmId)
        {
            return _dbContext.Films.Find(filmId);
        }

        /// <inheritdoc />
        public List<Film?> GetAllFilms()
        {
            return _dbContext.Films
                .ToList();
        }

        /// <inheritdoc />
        public List<Film?> GetAllFilmsByCategoryId(int topicId)
        {
            return _dbContext.Films
                .Where(q => q.CategoryId == topicId)
                .ToList();
        }

        /// <inheritdoc />
        public Film? GetRandomFilmFromCategoryByPrice(int topicId, int price)
        {
            Category topic = _topicService.GetCategoryById(topicId);

            if (topic.Films == null)
            {
                return null;
            }

            List<Film?> filmsWithPrice = topic.Films

                //.Where(q => q.Price == price)
                .ToList();

            if (filmsWithPrice.Count == 0)
            {
                throw new ArgumentException($"No films found for topic '{topic.Name}' with price {price}.");
            }

            int randomIndex = RandomGenerator.GetRandom().Next(0, filmsWithPrice.Count);

            return filmsWithPrice[randomIndex];
        }

        private readonly IKinopoiskDbManager _kinopoiskDbManager;
        private readonly ICategoryService _topicService;
        private readonly KinopoiskDbContext _dbContext;
    }
}