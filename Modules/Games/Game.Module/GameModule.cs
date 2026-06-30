using Game.Infrastructure.Interfaces.Services;
using Game.Infrastructure.Services;
using Game.Ui;
using Game.Ui.Views;
using Game.Ui.Views.Pages;
using Prism.Ioc;
using Prism.Modularity;

namespace Game.Module
{
    /// <summary>
    /// Модуль управления БД тем с вопросами
    /// </summary>
    public class GameModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry

                // сперва регистрируем контекст БД с вопросами
                /*.RegisterSingleton<IKinopoiskDbManager, KinopoiskDbManager>()
                .RegisterSingleton<ICategoryService, CategoryService>()
                .RegisterSingleton<IFilmService, FilmService>()
                .RegisterSingleton<IFilmImportService, FilmImportService>();*/
                .RegisterSingleton<IGameServices, GameServices>();

            // регистрируем View для навигации по Регионам
            containerRegistry.RegisterForNavigation<TestFilmView, TestFilmViewModel>();
            containerRegistry.RegisterForNavigation<GameMainLayersView, GameMainLayersViewModel>();
            containerRegistry.RegisterForNavigation<GameView, GameViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            //containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));
        }
    }
}