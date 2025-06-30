using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
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
    public class ImportFromFileViewModel : NavigationViewModelBase, IInitializable
    {
        /// <inheritdoc />
        public ImportFromFileViewModel(
            IRegionManager regionManager,
            ICategoryService categoryService,
            IFilmService filmService,
            IFilmImportService filmImportService)
            : base(regionManager)
        {
            _categoryService = categoryService;
            _filmService = filmService;
            _filmImportService = filmImportService;

            SelectFileCommand = new DelegateCommand<TopLevel>(async (o) => await OnSelectFile(o));

            CreateCommand = new DelegateCommand(async () => await OnCreate(), () => !Name.IsNullOrEmpty() && !Name.IsWhiteSpace())
                .ObservesProperty(() => Name);
        }

        public Category Category
        {
            get => _category;
            set => this.RaiseAndSetIfChanged(ref _category, value);
        }

        private Category _category;

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string? _name;

        public string? FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        private string? _fileName;

        public ICommand CreateCommand { get; }

        public DelegateCommand<TopLevel> SelectFileCommand { get; }

        /// <inheritdoc />
        public void Initialize()
        {
            Category = new Category();
        }

        /// <summary>
        /// Выбрать файл
        /// </summary>
        /// <param name="topLevel"></param>
        /// <returns></returns>
        private async Task OnSelectFile(TopLevel topLevel)
        {
            string? filePath = await GetFilePath(topLevel);

            if (filePath.IsNullOrEmpty())
            {
                return;
            }

            FileName = filePath;

            Category? item = await _filmImportService.ImportFromJsonAsync(filePath);

            if (item is null)
            {
                return;
            }

            Category = item;
            Name = item.Name;
        }

        private async Task OnCreate()
        {
            Category.Name = _name.Trim();

            await _categoryService.CreateCategoryAsync(Category);

            //await _filmService.AddFilms(Category.Films);

            MoveBackCommand.Execute(null);
        }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Initialize();
        }

        /// <inheritdoc />
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameters.Add(NavigationParameterService.ResultParameter, true);
            FileName = null;
            Name = null;
        }

        private async Task<string?> GetFilePath(TopLevel topLevel)
        {
            IList<string?>? dialogResult = await ShowOpenFileDialog(topLevel);

            // нажали кнопку "Отмена" или просто закрыли окно
            return dialogResult?.First();
        }

        /// <summary>
        /// Отобразить окно выбора файлов.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns>Список файлов. Если NULL - значит окно было закрыто пользователем.</returns>
        private async Task<IList<string?>?> ShowOpenFileDialog(TopLevel topLevel)
        {
            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider
                .OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    AllowMultiple = false,
                    FileTypeFilter = new List<FilePickerFileType>()
                    {
                        new FilePickerFileType("JSON Files")
                        {
                            Patterns = new List<string>()
                            {
                                "*.json"
                            }
                        },
                        FilePickerFileTypes.All
                    }
                });

            return files.Count switch
            {
                > 0 => files.Select(storageFile => storageFile.Path.LocalPath).ToList(),
                _ => null
            };
        }

        private readonly ICategoryService _categoryService;
        private readonly IFilmService _filmService;
        private readonly IFilmImportService _filmImportService;
    }
}
