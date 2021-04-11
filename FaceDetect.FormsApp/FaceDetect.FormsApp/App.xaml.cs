namespace FaceDetect.FormsApp
{
    using System;
    using System.IO;
    using System.Reflection;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Helpers;
    using FaceDetect.FormsApp.Modules;
    using FaceDetect.FormsApp.Services;
    using FaceDetect.FormsApp.State;
    using FaceDetect.FormsApp.Usecase;

    using Smart.Data.Mapper;
    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    using Xamarin.Essentials;
    using XamarinFormsComponents;

    public partial class App
    {
        private readonly SmartResolver resolver;

        private readonly Navigator navigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config DataMapper
            SqlMapperConfig.Default.ConfigureTypeHandlers(config =>
            {
                config[typeof(Guid)] = new GuidTypeHandler();
            });

            // Config Resolver
            resolver = CreateResolver(provider);
            ResolveProvider.Default.UseSmartResolver(resolver);

            // Config Navigator
            navigator = new NavigatorConfig()
                .UseFormsNavigationProvider()
                .UseResolver(resolver)
                .UseIdViewMapper(m => m.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes))
                .ToNavigator();
            navigator.Navigated += (_, args) =>
            {
                // for debug
                System.Diagnostics.Debug.WriteLine(
                    $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
            };

            // Show MainWindow
            MainPage = resolver.Get<MainPage>();
        }

        private SmartResolver CreateResolver(IComponentProvider provider)
        {
            var config = new ResolverConfig()
                .UseAutoBinding()
                .UseArrayBinding()
                .UseAssignableBinding()
                .UsePropertyInjector()
                .UsePageContextScope();

            config.UseXamarinFormsComponents(adapter =>
            {
                adapter.AddDialogs();
                adapter.AddJsonSerializer();
                adapter.AddSettings();
            });

            config.BindSingleton<INavigator>(_ => navigator);

            config.BindSingleton<ApplicationState>();

            config.BindSingleton<Configuration>();
            config.BindSingleton<Session>();

            config.BindSingleton(new DataServiceOptions
            {
                Path = Path.Combine(FileSystem.AppDataDirectory, "Mobile.db")
            });
            config.BindSingleton<DataService>();

            config.BindSingleton<FaceDetectUsecase>();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override async void OnStart()
        {
            var dialogs = resolver.Get<IApplicationDialog>();
            var dataService = resolver.Get<DataService>();

            // Crash report
            await CrashReportHelper.ShowReport();

            // Permission
            while (await Permissions.IsPermissionRequired())
            {
                await Permissions.RequestPermissions();

                if (await Permissions.IsPermissionRequired())
                {
                    await dialogs.Information("Permission required.");
                }
            }

            // Database
            await dataService.PrepareAsync();

            // Navigate
            await navigator.ForwardAsync(ViewId.Menu);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
