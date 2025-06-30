using FilmsDB.Domain;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Managers;
using FilmsDB.Infrastructure.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace FilmsDB.Infrastructure.Services
{
    /// <inheritdoc />
    public class CategoryService : ICategoryService
    {
        public CategoryService(IKinopoiskDbManager topicDbManager)
        {
            _topicDbManager = topicDbManager;
            _dbContext = _topicDbManager.DbContext;
        }

        /// <inheritdoc />
        public async Task CreateCategoryAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public void UpdateCategory(Category updatedCategory)
        {
            if (updatedCategory is null)
            {
                return;
            }

            UpdateCategory(updatedCategory.Id, updatedCategory);
        }

        /// <inheritdoc />
        public void DeleteCategory(Category topic)
        {
            if (_dbContext.Categories.Contains(topic))
            {
                DeleteCategory(topic.Id);
            }
        }

        public void DeleteCategory(int topicId)
        {
            Category topic = _dbContext.Categories.Find(topicId);

            if (topic == null)
            {
                return;
            }

            _dbContext.Categories.Remove(topic);
            _dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public Category GetCategoryById(int topicId)
        {
            return _dbContext.Categories.Find(topicId);
        }

        /// <inheritdoc />
        public List<Category> GetAllCategories()
        {
            return _dbContext.Categories
                .Include(t => t.Films)
                .ToList();
        }

        /// <inheritdoc />
        public int GetAllCategoriesCount() => _dbContext.Categories.Count();

        private void UpdateCategory(int topicId, Category updatedCategory)
        {
            Category topic = _dbContext.Categories
                .Find(topicId);

            if (topic == null)
            {
                return;
            }

            topic.Name = updatedCategory.Name;

            // обновление других свойств
            _dbContext.SaveChanges();
        }

        private readonly IKinopoiskDbManager _topicDbManager;
        private readonly KinopoiskDbContext _dbContext;
    }
}