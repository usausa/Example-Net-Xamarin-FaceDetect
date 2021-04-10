namespace FaceDetect.FormsApp.Modules.Detect
{
    using Smart.Navigation.Attributes;

    using Xamarin.CommunityToolkit.UI.Views;

    [View(ViewId.DetectCamera)]
    public partial class DetectCameraView
    {
        public DetectCameraView()
        {
            InitializeComponent();
        }

        // TODO
        private void CameraView_OnMediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            CaptureImage.Source = e.Image;
        }
    }
}
