namespace FaceDetect.FormsApp.Usecase
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Models.Result;
    using FaceDetect.FormsApp.State;

    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

    public class FaceDetectUsecase
    {
        private static readonly PropertyInfo[] EmotionValues = typeof(Emotion).GetProperties();

        private readonly Configuration configuration;

        private readonly IApplicationDialog dialog;

        public FaceDetectUsecase(
            Configuration configuration,
            IApplicationDialog dialog)
        {
            this.configuration = configuration;
            this.dialog = dialog;
        }

        public async ValueTask<DetectResult> DetectAsync(byte[] image)
        {
            try
            {
                using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
                await using var stream = new MemoryStream(image);

                // TODO
                var detectedFaces = await client.Face.DetectWithStreamWithHttpMessagesAsync(
                    stream,
                    returnFaceAttributes: new List<FaceAttributeType>
                    {
                        FaceAttributeType.Accessories,
                        FaceAttributeType.Age,
                        FaceAttributeType.Blur,
                        FaceAttributeType.Emotion,
                        FaceAttributeType.Exposure,
                        FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender,
                        FaceAttributeType.Glasses,
                        FaceAttributeType.Hair,
                        FaceAttributeType.HeadPose,
                        FaceAttributeType.Makeup,
                        FaceAttributeType.Noise,
                        FaceAttributeType.Occlusion,
                        FaceAttributeType.Smile
                    },
                    detectionModel: DetectionModel.Detection01,
                    recognitionModel: RecognitionModel.Recognition03);
                if (detectedFaces.Body.Count == 0)
                {
                    await dialog.Information("Detect failed.");
                    return null;
                }

                if (detectedFaces.Body.Count > 1)
                {
                    await dialog.Information($"Detect {detectedFaces.Body.Count} faces and result is 1st face.");
                }

                var result = new DetectResult();
                var face = detectedFaces.Body[0];

                result.Age = face.FaceAttributes.Age;
                result.Gender = face.FaceAttributes.Gender.ToString();
                result.PredominantEmotion = DetectEmotion(face.FaceAttributes.Emotion);
                // TODO

                return result;
            }
            catch (Exception e)
            {
                await dialog.Information("Error", e.ToString());
                return null;
            }
        }

        private static string DetectEmotion(Emotion emotion)
        {
            var max = 0d;
            var maxEmotion = default(PropertyInfo);

            foreach (var pi in EmotionValues)
            {
                var value = (double)pi.GetValue(emotion);
                if (value > max)
                {
                    max = value;
                    maxEmotion = pi;
                }
            }

            return maxEmotion.Name;
        }
    }
}
