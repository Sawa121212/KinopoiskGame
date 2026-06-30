using FilmsDB.Domain.Models;

namespace FilmsDB.Infrastructure.Interfaces.Services
{
    /// <summary>
    /// для управления
    /// </summary>
    public interface IFilmService
    {
        Task AddFilmsAsync(List<Film?> categoryFilms);

        /// <summary>
        /// Создание нового вопроса
        /// </summary>
        /// <param name="question"></param>
        public Film CreateFilm(Film? question);

        /// <summary>
        /// Редактирование существующего вопроса
        /// </summary>
        /// <param name="updatedFilm"></param>
        void UpdateFilm(Film updatedFilm);

        /// <summary>
        /// Редактирование существующего вопроса
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="updatedFilm"></param>
        void UpdateFilm(int questionId, Film updatedFilm);

        /// <summary>
        /// Удаление вопроса
        /// </summary>
        /// <param name="question">Вопрос</param>
        void DeleteFilm(Film? question);

        /// <summary>
        /// Удаление вопроса
        /// </summary>
        /// <param name="questionId">ID вопроса</param>
        void DeleteFilm(int questionId);

        /// <summary>
        /// Получение вопроса по идентификатору
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Film? GetFilmById(int questionId);

        /// <summary>
        /// Получение всех
        /// </summary>
        /// <returns></returns>
        List<Film?> GetAllFilms();

        /// <summary>
        /// Получение всех вопросов по идентификатору темы
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        List<Film?> GetAllFilmsByCategoryId(int topicId);

        /// <summary>
        /// Получить случайный вопрос из темы по указанной цене
        /// </summary>
        /// <param name="topicId">ИД темы</param>
        /// <param name="questionBasePrice">Цена вопроса</param>
        /// <returns></returns>
        Film GetRandomFilmFromCategoryByPrice(int topicId, int questionBasePrice);
    }
}
