namespace FaceDetect.FormsApp.Modules
{
    using FaceDetect.FormsApp.Messaging;

    using Smart.Navigation;

    public static class Parameters
    {
        private const string Capture = nameof(Capture);

        public static NavigationParameter MakeCapture(CaptureResult result) =>
            new NavigationParameter().SetValue(Capture, result);

        public static CaptureResult GetCapture(this INavigationParameter parameter) =>
            parameter.GetValue<CaptureResult>(Capture);
    }
}
