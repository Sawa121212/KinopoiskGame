using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Common.Core.Prism;
using Common.Core.Prism.Regions;
using Common.Core.Views;
using Common.Extensions;
using FilmsDB.Domain.Models;
using FilmsDB.Infrastructure.Interfaces.Services;
using Notification.Module.Services;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;
using File = System.IO.File;

namespace FilmsDB.Ui.Views.Films
{
    public class AddNewFilmViewModel : NavigationViewModelBase
    {
        /// <inheritdoc />
        public AddNewFilmViewModel(
            IRegionManager regionManager,
            INotificationService notificationService,
            IFilmImportService filmImportService,
            IFilmService filmService)
            : base(regionManager)
        {
            _notificationService = notificationService;
            _filmImportService = filmImportService;
            _filmService = filmService;

            SelectFileCommand = new DelegateCommand<TopLevel>(async (o) => await OnSelectFile(o));

            CreateCommand = new DelegateCommand(async () => await OnDone());
        }

        public Category DataBaseCategory
        {
            get => _dataBaseCategory;
            set => this.RaiseAndSetIfChanged(ref _dataBaseCategory, value);
        }
        private Category _dataBaseCategory;

        public string? FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }
        private string? _fileName;

        public ObservableCollection<Film?> NewFilms
        {
            get => _newFilms;
            set => this.RaiseAndSetIfChanged(ref _newFilms, value);
        }
        private ObservableCollection<Film?> _newFilms;

        public ICommand CreateCommand { get; }

        public DelegateCommand<TopLevel> SelectFileCommand { get; }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            // Initialize parameter
            object? resultParameter = navigationContext.Parameters[NavigationParameterService.InitializeParameter];

            if (resultParameter is Category category)
            {
                DataBaseCategory = category;
            }
        }

        /// <inheritdoc />
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameters.Add(NavigationParameterService.ResultParameter, true);
            FileName = null;
        }

        /// <inheritdoc />
        protected override void GoBackOrder()
        {
            RegionManager.RequestNavigate(RegionNameService.ShellRegionName, "MainView");
            base.GoBackOrder();
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

            Category? fileCategory = await _filmImportService.ImportFromJsonAsync(filePath);

            if (fileCategory is null)
            {
                return;
            }

            NewFilms = new ObservableCollection<Film>(fileCategory.Films);
        }

        private async Task OnDone()
        {
            foreach (Film film in _newFilms)
            {
                film.Category = _dataBaseCategory;
                film.CategoryId = _dataBaseCategory.Id;

                _dataBaseCategory.Films.Add(film);
            }

            await _filmService.AddFilmsAsync(_dataBaseCategory.Films);
            MoveBackCommand.Execute(null);
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

        private Category _importedCategory;

        private readonly INotificationService _notificationService;
        private readonly IFilmService _filmService;

        private readonly ICategoryService _categoryService;
        private readonly IFilmImportService _filmImportService;
    }
}