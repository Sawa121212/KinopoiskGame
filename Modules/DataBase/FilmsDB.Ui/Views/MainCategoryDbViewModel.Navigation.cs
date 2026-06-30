using Common.Core.Prism;
using Common.Core.Prism.Regions;
using Confirmation.Module.Enums;
using FilmsDB.Domain.Models;
using FilmsDB.Ui.Views.Categories;
using FilmsDB.Ui.Views.Categories.Creating;
using FilmsDB.Ui.Views.Categories.ImportingFile;
using FilmsDB.Ui.Views.Films;
using Prism.Commands;
using Prism.Regions;

namespace FilmsDB.Ui.Views
{
    public partial class MainCategoryDbViewModel
    {
        private void OnImportFromFile()
        {
            RegionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(ImportFromFileView));
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnAddNewCategory()
        {
            RegionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(AddNewCategoryView));
        }

        private void OnEditCategory(Category category)
        {
            NavigationParameters parameter = new()
            {
                {
                    NavigationParameterService.InitializeParameter, category
                }
            };

            RegionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(AddNewCategoryView), parameter);
        }

        private async void OnDeleteCategory(Category category)
        {
            ConfirmationResultEnum result = await _confirmationService.ShowInfoAsync(
                    "Подтверждение",
                    $"Вы действительно хотите удалить тему \"{category.Name}\"?",
                    ConfirmationResultEnum.Yes | ConfirmationResultEnum.No)
                .ConfigureAwait(true);

            if (result == ConfirmationResultEnum.Yes)
            {
                _categoryService.DeleteCategory(category);
                UpdateCategoriesInformation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        private void OnAddNewFilm(Category category)
        {
            NavigationParameters parameter = new()
            {
                {
                    NavigationParameterService.InitializeParameter, category
                }
            };

            RegionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(AddNewFilmView), parameter);
        }

        private void OnEditFilm(Film film)
        {
            NavigationParameters parameter = new()
            {
                {
                    NavigationParameterService.InitializeParameter, film
                }
            };

            RegionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(AddNewFilmView), parameter);
        }

        private async void OnDeleteFilm(Film? film)
        {
            ConfirmationResultEnum result = await _confirmationService.ShowInfoAsync("Подтверждение",
                $"Вы действительно хотите удалить вопрос?",
                ConfirmationResultEnum.Yes | ConfirmationResultEnum.No);

            if (result == ConfirmationResultEnum.Yes)
            {
                _filmService.DeleteFilm(film);
            }
        }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            object parameter = navigationContext.Parameters[NavigationParameterService.ResultParameter];

            if (parameter is bool and true)
            {
                UpdateCategoriesInformation();
            }

            AddNewCategoryCommand.RaiseCanExecuteChanged();
            EditCategoryCommand.RaiseCanExecuteChanged();
        }

        /// <inheritdoc />
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public DelegateCommand AddNewCategoryCommand { get; }

        public DelegateCommand<Category> EditCategoryCommand { get; }

        public DelegateCommand<Category> DeleteCategoryCommand { get; }

        public DelegateCommand<Category> AddNewFilmCommand { get; }

        public DelegateCommand<Film> EditFilmCommand { get; }

        public DelegateCommand<Film> DeleteFilmCommand { get; }
    }
}
