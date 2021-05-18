namespace FaceDetect.FormsApp.Models.Result
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    public class DetectResult
    {
        public double? Age { get; set; }

        public string? Gender { get; set; }

        [AllowNull]
        public string PredominantEmotion { get; set; }

        [AllowNull]
        public string Hair { get; set; }

        [AllowNull]
        public string[] Accessories { get; set; }

        public string? Glasses { get; set; }

        public double? Smile { get; set; }

        public Rectangle FaceRectangle { get; set; }
    }
}
