using FilmsDB.Domain.Models;

namespace FilmsDB.Infrastructure.Interfaces.Services
{
    /// <summary>
    /// для управления
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Создание новой темы
        /// </summary>
        /// <param name="category"></param>
        public Task CreateCategoryAsync(Category category);

        /// <summary>
        /// Редактирование существующей темы
        /// </summary>
        /// <param name="updatedCategory"></param>
        public void UpdateCategory(Category updatedCategory);

        /// <summary>
        /// Удаление темы
        /// </summary>
        /// <param name="category">Тема</param>
        public void DeleteCategory(Category category);

        /// <summary>
        /// Получение темы по идентификатору
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId);

        /// <summary>
        /// Получение всех тем
        /// </summary>
        /// <returns></returns>
        public List<Category> GetAllCategories();

        /// <summary>
        /// Получить количество всех тем
        /// </summary>
        /// <returns></returns>
        int GetAllCategoriesCount();
    }
}