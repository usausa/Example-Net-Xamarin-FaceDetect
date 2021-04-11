namespace FaceDetect.FormsApp.Modules
{
    using Smart.Navigation;

    using Xamarin.Forms;

    public static class Parameters
    {
        private const string Image = nameof(Image);

        private const string Rotation = nameof(Rotation);

        public static NavigationParameter MakeImage(ImageSource image, double rotation) =>
            new NavigationParameter().SetValue(Image, image).SetValue(Rotation, rotation);

        public static ImageSource GetImage(this INavigationParameter parameter) =>
            parameter.GetValue<ImageSource>(Image);

        public static double GetRotation(this INavigationParameter parameter) =>
            parameter.GetValue<double>(Rotation);
    }
}
