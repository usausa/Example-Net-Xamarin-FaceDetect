namespace FaceDetect.FormsApp.Usecase
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Models.Result;

    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

    public class FaceDetectUsecase
    {
        private const string Key = "()";
        private const string EndPoint = "()";

        public async ValueTask<DetectResult> DetectAsync(byte[] image)
        {
            using var client = new FaceClient(new ApiKeyServiceClientCredentials(Key)) { Endpoint = EndPoint };
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

            return detectedFaces.Body.Count > 0 ? new DetectResult() : null;
        }
    }
}
