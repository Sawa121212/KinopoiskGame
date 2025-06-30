using System.Windows.Input;
using Common.Core.Interfaces;
using Common.Core.Prism;
using Common.Core.Views;
using Common.Extensions;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Services;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

namespace FilmsDB.Ui.Views.Categories
{
    public class AddNewCategoryViewModel : NavigationViewModelBase, IInitializable
    {
        /// <inheritdoc />
        public AddNewCategoryViewModel(
            IRegionManager regionManager,
            ICategoryService CategoryService)
            : base(regionManager)
        {
            _categoryService = CategoryService;

            CreateCommand = new DelegateCommand(OnCreate, () => !Name.IsNullOrEmpty() && !Name.IsWhiteSpace())
                .ObservesProperty(() => Name);
        }

        public Category Category
        {
            get => _category;
            set => this.RaiseAndSetIfChanged(ref _category, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public bool IsCreateMode { get; set; } = true;

        public ICommand CreateCommand { get; }

        /// <inheritdoc />
        public void Initialize()
        {
            Category = new Category();
        }

        private void OnCreate()
        {
            Category.Name = _name.Trim();

            if (IsCreateMode)
            {
                _categoryService.CreateCategoryAsync(_category);
            }
            else
            {
                _categoryService.UpdateCategory(_category);
            }

            MoveBackCommand.Execute(null);
        }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            object id = navigationContext.Parameters[NavigationParameterService.InitializeParameter];

            if (id is Category Category)
            {
                Category = Category;
                Name = Category.Name;
                IsCreateMode = false;
            }
            else
            {
                Initialize();
            }
        }

        /// <inheritdoc />
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameters.Add(NavigationParameterService.ResultParameter, true);

            Name = default;
        }

        private Category _category;
        private string _name;
        private readonly ICategoryService _categoryService;
    }
}
