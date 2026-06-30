using Common.Core.Prism;
using Game.Domain.Models;
using Prism.Regions;

namespace Game.Ui.Views.Pages
{
    public partial class GameViewModel
    {
        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            ClearAllParameters();

            // Initialize parameter
            object? resultParameter = navigationContext.Parameters[NavigationParameterService.InitializeParameter];

            if (resultParameter is GameMode gameMode)
            {
                _gameMode = gameMode;
            }

            if (_gameMode?.GameFilmsCount == 0)
            {
                throw new Exception();
            }

            _filmsEnumerator = _gameMode.GameFilms.GetEnumerator();

            ShowNextFilmCommand.Execute(_gameMode);
        }

        protected override async Task GoBackOrderAsync()
        {
            /*if (IsGameStarted)
            {
                ConfirmationResultEnum result = await _confirmationService.ShowInfoAsync(
                    "Подтверждение",
                    "Хотите вернутся в комнату? Игра будет завершена.",
                    ConfirmationResultEnum.Yes | ConfirmationResultEnum.No);

                if (result == ConfirmationResultEnum.Yes)
                {
                    _gameManager.CloseGame();
                    ClearAllParameters();
                }

                Prism.Regions.RegionManager.RequestNavigate(GameRegionNameService.GameMainLayerRegionName, nameof(RoomView));

                return;
            }*/

            MoveBackCommand.Execute(default);
        }
    }
}