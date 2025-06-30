using System.Collections.ObjectModel;
using Common.Core.Prism.Regions;
using Common.Core.Views;
using Confirmation.Module.Services;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Services;
using FilmsDB.Ui.Views.Categories;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

namespace FilmsDB.Ui.Views
{
    public partial class MainCategoryDbViewModel : NavigationViewModelBase
    {
        public MainCategoryDbViewModel(
            IRegionManager regionManager,
            IConfirmationService confirmationService,
            ICategoryService CategoryService,
            IFilmService FilmService)
            : base(regionManager)
        {
            _confirmationService = confirmationService;
            _categoryService = CategoryService;
            _filmService = FilmService;
            Categories = new ObservableCollection<Category>();
            Films = new ObservableCollection<Film>();

            // Category commands
            AddNewCategoryCommand = new DelegateCommand(OnAddNewCategory);
            EditCategoryCommand = new DelegateCommand<Category>(OnEditCategory);
            DeleteCategoryCommand = new DelegateCommand<Category>(OnDeleteCategory);

            //Film commands
            AddNewFilmCommand = new DelegateCommand<Category>(OnAddNewFilm);
            EditFilmCommand = new DelegateCommand<Film>(OnEditFilm);
            DeleteFilmCommand = new DelegateCommand<Film>(OnDeleteFilm);

            // search commands
            FindCategoriesCommand = new DelegateCommand(OnFindCategories);
            ClearFoundElementsCommand = new DelegateCommand(OnClearFoundElements);
            UpdateCategoriesInformation();

            ImportFromFileCommand = new DelegateCommand(OnImportFromFile);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => this.RaiseAndSetIfChanged(ref _categories, value);
        }

        private ObservableCollection<Category> _categories;

        public ObservableCollection<Film> Films
        {
            get => _films;
            set => this.RaiseAndSetIfChanged(ref _films, value);
        }

        private ObservableCollection<Film> _films;

        public DelegateCommand ImportFromFileCommand { get; }

        private void UpdateCategoriesInformation()
        {
            Categories.Clear();

            foreach (Category customer in _categoryService.GetAllCategories())
            {
                Categories.Add(customer);
            }

            Films.Clear();
            Films.AddRange(_filmService.GetAllFilms());
        }

        private readonly IConfirmationService _confirmationService;
        private readonly ICategoryService _categoryService;
        private readonly IFilmService _filmService;
    }
}
