namespace FaceDetect.FormsApp
{
    using System.Reflection;
    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Modules;
    using FaceDetect.FormsApp.Services;
    using FaceDetect.FormsApp.State;
    using FaceDetect.FormsApp.Usecase;
    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    using XamarinFormsComponents;

    public partial class App
    {
        private readonly SmartResolver resolver;

        private readonly Navigator navigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

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

            config.BindSingleton<DataService>();

            config.BindSingleton<FaceDetectUsecase>();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override async void OnStart()
        {
            var dialogs = resolver.Get<IApplicationDialog>();

            // Permission
            while (await Permissions.IsPermissionRequired())
            {
                await Permissions.RequestPermissions();

                if (await Permissions.IsPermissionRequired())
                {
                    await dialogs.Information("Permission required.");
                }
            }

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
