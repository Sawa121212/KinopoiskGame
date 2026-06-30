using Common.Core.Localization;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace KinopoiskGame.Views
{
    public partial class MainViewModel : BindableBase
    {
        public MainViewModel(IRegionManager regionManager, ILocalizer localizer)
        {
            _localizer = localizer;
            _regionManager = regionManager;

            ShowCategoriesCommand = new DelegateCommand(OnShowCategories);
            ShowPlayInformationCommand = new DelegateCommand(OnShowPlayInformation);
            ShowSettingsCommand = new DelegateCommand(OnShowSettings);
            ShowTestFilmCommand = new DelegateCommand(OnShowTestFilm);
            PlayCommand = new DelegateCommand(OnPlay);
        }

        private readonly ILocalizer _localizer;
    }
}