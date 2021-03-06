namespace FaceDetect.FormsApp.Droid;

using System.Threading.Tasks;

using Acr.UserDialogs;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;

using FaceDetect.FormsApp.Components.Dialog;
using FaceDetect.FormsApp.Droid.Components.Dialog;
using FaceDetect.FormsApp.Helpers;

using Smart.Forms.Resolver;
using Smart.Resolver;

using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

[Activity(
    Name = "FaceDetect.app.MainActivity",
    Icon = "@mipmap/icon",
    Theme = "@style/MainTheme.Splash",
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
    ScreenOrientation = ScreenOrientation.Portrait,
    WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        SetTheme(Resource.Style.MainTheme);
        base.OnCreate(savedInstanceState);

        // Setup crash report
        TaskScheduler.UnobservedTaskException += (_, args) =>
            CrashReportHelper.LogException(args.Exception);
        AndroidEnvironment.UnhandledExceptionRaiser += (_, args) =>
            CrashReportHelper.LogException(args.Exception);

        // Database
        SQLitePCL.Batteries_V2.Init();

        // Barcode
        ZXing.Net.Mobile.Forms.Android.Platform.Init();

        // Components
        UserDialogs.Init(this);

        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        Xamarin.Forms.Forms.Init(this, savedInstanceState);

        // Boot
        LoadApplication(new App(new ComponentProvider(this)));

        Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>()
            .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    private sealed class ComponentProvider : IComponentProvider
    {
        private readonly MainActivity activity;

        public ComponentProvider(MainActivity activity)
        {
            this.activity = activity;
        }

        public void RegisterComponents(ResolverConfig config)
        {
            config.Bind<Activity>().ToConstant(activity).InSingletonScope();

            config.Bind<IApplicationDialog>().To<ApplicationDialog>().InSingletonScope();
        }
    }
}
