using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.Core.Prism;
using Common.Core.Prism.Regions;
using Common.Core.Views;
using Game.Domain.Models;
using Game.Infrastructure.Interfaces.Services;
using Game.Ui.Views.Pages;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

namespace Game.Ui.Views
{
    public class GameMainLayersViewModel : NavigationViewModelBase
    {
        /// <inheritdoc />
        public GameMainLayersViewModel(
            IRegionManager regionManager,
            IGameServices gameServices)
            : base(regionManager)
        {
            _gameServices = gameServices;

            SelectGameModeCommand = new DelegateCommand<GameMode>(OnSelectGameMode);

            Initialize();
        }

        public ObservableCollection<GameMode> GameModes
        {
            get => _gameModes;
            set => this.RaiseAndSetIfChanged(ref _gameModes, value);
        }

        public ICommand SelectGameModeCommand { get; }

        private void Initialize()
        {
            GameModes = new ObservableCollection<GameMode>(_gameServices.CreateGameModes());
        }

        private void OnSelectGameMode(GameMode? gameMode)
        {
            if (gameMode is null)
            {
                return;
            }

            gameMode.CreateGameFilms(10);

            NavigationParameters parameter = new()
            {
                {
                    NavigationParameterService.InitializeParameter, gameMode
                }
            };

            RegionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(GameView), parameter);
        }

        private ObservableCollection<GameMode> _gameModes;

        private readonly IGameServices _gameServices;
    }
}