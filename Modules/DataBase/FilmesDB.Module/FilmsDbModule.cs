using FilmsDB.Infrastructure.Interfaces.Managers;
using FilmsDB.Infrastructure.Interfaces.Services;
using FilmsDB.Infrastructure.Managers;
using FilmsDB.Infrastructure.Services;
using FilmsDB.Ui.Views;
using FilmsDB.Ui.Views.Categories;
using FilmsDB.Ui.Views.Films;
using Prism.Ioc;
using Prism.Modularity;

namespace FilmsDB.Module
{
    /// <summary>
    /// Модуль управления БД тем с вопросами
    /// </summary>
    public class FilmsDbModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry

                // сперва регистрируем контекст БД с вопросами
                .RegisterSingleton<IKinopoiskDbManager, KinopoiskDbManager>()
                .RegisterSingleton<ICategoryService, CategoryService>()
                .RegisterSingleton<IFilmService, FilmService>()
                .RegisterSingleton<IFilmImportService, FilmImportService>();

            // регистрируем View для навигации по Регионам
            containerRegistry.RegisterForNavigation<MainCategoryDbView, MainCategoryDbViewModel>();
            containerRegistry.RegisterForNavigation<AddNewCategoryView, AddNewCategoryViewModel>();
            containerRegistry.RegisterForNavigation<AddNewFilmView, AddNewFilmViewModel>();
            containerRegistry.RegisterForNavigation<ImportFromFileView, ImportFromFileViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            //containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));
        }
    }
}
