using System;
using System.Reflection;
using System.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Common.Core.Localization;
using Common.Core.Prism.Regions;
using Confirmation.Module;
using Confirmation.Module.Services;
using FilmsDB.Module;
using Game.Module;
using Infrastructure.Environment.Managers;
using Infrastructure.Environment.Services;
using Infrastructure.Environment.Services.ApplicationInfo;
using Infrastructure.Environment.Services.Settings;
using Infrastructure.Interfaces.Managers;
using Infrastructure.Interfaces.Services;
using Infrastructure.Interfaces.Services.Settings;
using Infrastructure.Module;
using KinopoiskGame.Properties;
using KinopoiskGame.Views;
using KinopoiskGame.Views.PlayInfoPages;
using KinopoiskGame.Views.Shell;
using Notification.Module;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using IResourceProvider = Common.Core.Localization.IResourceProvider;
using ResourceProvider = KinopoiskGame.Properties.ResourceProvider;

namespace KinopoiskGame
{
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            // Initializes Prism.Avalonia
            base.Initialize();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        /// <summary>
        /// Регистрация служб приложения
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry

                // Register Services
                .RegisterSingleton<ILocalizer, Localizer>()
                .RegisterSingleton<IResourceProvider, ResourceProvider>(Assembly.GetExecutingAssembly().FullName)
                .RegisterSingleton<IResourceService, ResourceService>()

                // .RegisterSingleton<IPresentationParameters, PresentationParameters>()

                // InfrastructureModule
                .RegisterSingleton<ISerializableSettingsManager, SerializableSettingsManager>()
                .RegisterSingleton<IApplicationInfoService, ApplicationInfoService>()
                .RegisterSingleton<ISettingsViewManager, SettingsViewManager>()
                .RegisterSingleton<IPathService, PathService>()
                .RegisterSingleton<IProtobufSerializeService, ProtobufSerializeService>()
                .RegisterSingleton<IApplicationSettingsService, ApplicationSettingsService>()
                .RegisterSingleton<IApplicationSettingsRepositoryService, ApplicationSettingsRepositoryService>()
                .RegisterSingleton<IConfirmationService, ConfirmationService>()

                // сперва регистрируем контекст БД с вопросам
                // .RegisterScoped<UserDbContext>()
                // .RegisterSingleton<IUserDbManager, UserDbManager>()
                // .RegisterSingleton<IUserService, UserService>()
                ;

            // Views - Generic
            containerRegistry.Register<ShellView>();

            // Views - Region Navigation
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<PlayInfoView>();
        }

        /// <summary>
        /// Регистрация модулей приложения
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog

                // App
                .AddModule<ConfirmationModule>()
                .AddModule<NotificationModule>()
                .AddModule<InfrastructureModule>()

                // modules
                .AddModule<FilmsDbModule>()
                .AddModule<GameModule>()

                // .AddModule<UsersModule>()
                // .AddModule<GameSenderModule>()
                ;
        }

        protected override void InitializeShell(AvaloniaObject shell)
        {
            // Добавим локализацию
            Container.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));

            base.InitializeShell(shell);
        }

        /// <summary>Called after <seealso cref="Initialize"/>.</summary>
        protected override void OnInitialized()
        {
            Container.Resolve<IRegionManager>()
                .RegisterViewWithRegion(RegionNameService.ShellRegionName, nameof(MainView))
                .RegisterViewWithRegion(RegionNameService.ContentRegionName, nameof(PlayInfoView))
                ;
        }

        /// <summary>
        /// ViewModel Locator
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                string? viewName = viewType.FullName;
                string? viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                string viewModelName = string.Format(
                    viewName != null && viewName.EndsWith("View", StringComparison.OrdinalIgnoreCase)
                        ? "{0}Model, {1}"
                        : "{0}ViewModel, {1}",
                    viewName, viewAssemblyName);

                return Type.GetType(viewModelName);
            });

            ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));
        }
    }
}
