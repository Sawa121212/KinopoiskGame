using System.Collections.ObjectModel;
using System.Windows.Input;
using FilmsDB.Domain.Models;
using ReactiveUI;

namespace FilmsDB.Ui.Views
{
    public partial class MainCategoryDbViewModel
    {
        private string _filterText;

        private void OnFindCategories()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                return;
            }

            string searchText = FilterText.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            List<Category> allCategorys = _categoryService.GetAllCategories();

            if (!allCategorys.Any())
            {
                return;
            }

            List<Category> foundedElements = new(allCategorys.Where(o => o.Name != null && o.Name.ToLower().Contains(searchText)));

            if (!foundedElements.Any())
            {
                return;
            }

            Categories.Clear();
            Categories.AddRange(foundedElements);
        }

        private void OnClearFoundElements()
        {
            FilterText = string.Empty;
            Categories = new ObservableCollection<Category>(_categoryService.GetAllCategories());
        }

        public string FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }

        public ICommand FindCategoriesCommand { get; }

        public ICommand ClearFoundElementsCommand { get; }
    }
}
