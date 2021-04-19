namespace FaceDetect.FormsApp.Modules
{
    using FaceDetect.FormsApp.Models.Entity;

    using Smart.Navigation;

    public static class Parameters
    {
        private const string Image = nameof(Image);

        private const string Person = nameof(Person);

        public static NavigationParameter MakeImage(byte[] data) =>
            new NavigationParameter().SetValue(Image, data);

        public static byte[] GetImage(this INavigationParameter parameter) =>
            parameter.GetValue<byte[]>(Image);

        public static NavigationParameter MakePerson(PersonEntity entity) =>
            new NavigationParameter().SetValue(Person, entity);

        public static PersonEntity GetPerson(this INavigationParameter parameter) =>
            parameter.GetValue<PersonEntity>(Person);

        public static PersonEntity GetPersonOrDefault(this INavigationParameter parameter) =>
            parameter.GetValueOrDefault<PersonEntity>(Person);
    }
}
