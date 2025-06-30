using System.Windows.Input;
using Common.Core.Prism.Regions;
using FilmsDB.Ui.Views;
using Game.Ui.Views;
using Infrastructure.Ui.Views;
using KinopoiskGame.Views.PlayInfoPages;
using Prism.Regions;

namespace KinopoiskGame.Views
{
    public partial class MainViewModel
    {
        public ICommand ShowCategoriesCommand { get; }

        public ICommand ShowPlayInformationCommand { get; }

        public ICommand ShowSettingsCommand { get; }

        public ICommand PlayCommand { get; }

        public ICommand ShowTestFilmCommand { get; }

        private void OnShowCategories()
        {
            _regionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(MainCategoryDbView));
        }

        private void OnShowPlayInformation()
        {
            _regionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(PlayInfoView));
        }

        private void OnPlay()
        {
            _regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(GameMainLayersView));
        }

        private void OnShowSettings()
        {
            _regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(SettingsView));
        }

        private readonly IRegionManager _regionManager;

        private void OnShowTestFilm()
        {
            _regionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(TestFilmView));
        }
    }
}
