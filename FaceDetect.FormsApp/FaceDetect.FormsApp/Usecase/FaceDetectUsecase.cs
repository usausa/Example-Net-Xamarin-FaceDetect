namespace FaceDetect.FormsApp.Usecase
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Models.Result;
    using FaceDetect.FormsApp.Services;
    using FaceDetect.FormsApp.State;

    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

    public class FaceDetectUsecase
    {
        private static readonly PropertyInfo[] EmotionValues = typeof(Emotion).GetProperties();

        private readonly Configuration configuration;

        private readonly IApplicationDialog dialog;

        private readonly DataService dataService;

        public FaceDetectUsecase(
            Configuration configuration,
            IApplicationDialog dialog,
            DataService dataService)
        {
            this.configuration = configuration;
            this.dialog = dialog;
            this.dataService = dataService;
        }

        //--------------------------------------------------------------------------------
        // Detect
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "Ignore")]
        public async ValueTask<DetectResult> DetectAsync(byte[] image)
        {
            try
            {
                using var loading = dialog.Loading("Detecting");
                using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
                await using var stream = new MemoryStream(image);

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
                    loading.Dispose();
                    await dialog.Information("Detect failed.");
                    return null;
                }

                if (detectedFaces.Body.Count > 1)
                {
                    loading.Dispose();
                    await dialog.Information($"Detect {detectedFaces.Body.Count} faces and result is 1st face.");
                }

                var result = new DetectResult();
                var face = detectedFaces.Body[0];

                result.Age = face.FaceAttributes.Age;
                result.Gender = face.FaceAttributes.Gender.ToString();
                result.PredominantEmotion = DetectEmotion(face.FaceAttributes.Emotion);
                result.Hair = DetectHair(face.FaceAttributes.Hair);
                result.Accessories = face.FaceAttributes.Accessories.Select(x => x.Type.ToString()).ToArray();
                result.Glasses = face.FaceAttributes.Glasses?.ToString();
                result.Smile = face.FaceAttributes.Smile;
                result.FaceRectangle = new Rectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);

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

        private static string DetectHair(Hair hair)
        {
            if (hair.HairColor.Count == 0)
            {
                return hair.Invisible ? "Invisible" : "Bald";
            }

            var max = 0d;
            var maxColor = default(string);
            foreach (var hairColor in hair.HairColor)
            {
                if (hairColor.Confidence > max)
                {
                    max = hairColor.Confidence;
                    maxColor = hairColor.Color.ToString();
                }
            }

            return maxColor;
        }

        //--------------------------------------------------------------------------------
        // Person
        //--------------------------------------------------------------------------------

        public async ValueTask CreatePersonAsync(string name)
        {
            using var loading = dialog.Loading("Creating");
            using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
            var groupId = await PrepareGroupAsync(client, RecognitionModel.Recognition04);

            var person = await client.PersonGroupPerson.CreateAsync(groupId, name);

            await dataService.InsertPerson(new PersonEntity { Id = person.PersonId,  Name = name });
        }

        public async ValueTask UpdatePersonAsync(PersonEntity person)
        {
            using var loading = dialog.Loading("Updating");
            using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
            var groupId = await PrepareGroupAsync(client, RecognitionModel.Recognition04);

            await client.PersonGroupPerson.UpdateAsync(groupId, person.Id, person.Name);
        }

        public async ValueTask DeletePersonAsync(Guid personId)
        {
            using var loading = dialog.Loading("Deleting");
            using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
            var groupId = await PrepareGroupAsync(client, RecognitionModel.Recognition04);

            await client.PersonGroupPerson.DeleteAsync(groupId, personId);

            await dataService.DeletePerson(personId);
        }

        //--------------------------------------------------------------------------------
        // Learn
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "Ignore")]
        public async ValueTask<bool> LearnAsync(Guid personId, byte[] image)
        {
            try
            {
                using var loading = dialog.Loading("Learning");
                using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
                await using var stream = new MemoryStream(image);

                var groupId = await PrepareGroupAsync(client, RecognitionModel.Recognition04);

                await client.PersonGroupPerson.AddFaceFromStreamWithHttpMessagesAsync(
                    groupId,
                    personId,
                    stream,
                    detectionModel: DetectionModel.Detection03);

                await client.PersonGroup.TrainAsync(groupId);

                var watch = Stopwatch.StartNew();
                while (true)
                {
                    await Task.Delay(500);

                    var status = await client.PersonGroup.GetTrainingStatusAsync(groupId);
                    if (status.Status == TrainingStatusType.Succeeded)
                    {
                        break;
                    }

                    if (status.Status == TrainingStatusType.Failed)
                    {
                        loading.Dispose();
                        await dialog.Information("Learning failed.");
                        return false;
                    }

                    if (watch.ElapsedMilliseconds > 10_000)
                    {
                        loading.Dispose();
                        await dialog.Information("Learning timeout.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                await dialog.Information("Error", e.ToString());
                return false;
            }
        }

        //--------------------------------------------------------------------------------
        // Identify
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "Ignore")]
        public async ValueTask<FaceDetect.FormsApp.Models.Result.IdentifyResult> IdentifyAsync(byte[] image)
        {
            try
            {
                using var loading = dialog.Loading("Identifying");
                using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };
                await using var stream = new MemoryStream(image);

                var groupId = await PrepareGroupAsync(client, RecognitionModel.Recognition04);

                var faces = await client.Face.DetectWithStreamWithHttpMessagesAsync(
                    stream,
                    recognitionModel: RecognitionModel.Recognition04,
                    detectionModel: DetectionModel.Detection03);
                if (faces.Body.Count == 0)
                {
                    loading.Dispose();
                    await dialog.Information("Face detect failed.");
                    return null;
                }

                if (faces.Body.Count > 1)
                {
                    loading.Dispose();
                    await dialog.Information("Multiple people detected.");
                    return null;
                }

                var face = faces.Body[0];

                // ReSharper disable once PossibleInvalidOperationException
                var faceIds = new List<Guid>
                {
                    face.FaceId.Value
                };

                var identifyResults = await client.Face.IdentifyAsync(faceIds, groupId);
                if (identifyResults.Count == 0)
                {
                    loading.Dispose();
                    await dialog.Information("Identify failed.");
                    return null;
                }

                var person = await client.PersonGroupPerson.GetAsync(groupId, identifyResults[0].Candidates[0].PersonId);

                return new FaceDetect.FormsApp.Models.Result.IdentifyResult
                {
                    Id = person.PersonId,
                    Confidence = identifyResults[0].Candidates[0].Confidence,
                    FaceRectangle = new Rectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height)
                };
            }
            catch (Exception e)
            {
                await dialog.Information("Error", e.ToString());
                return null;
            }
        }

        //--------------------------------------------------------------------------------
        // Reset
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "Ignore")]
        public async ValueTask ResetAsync()
        {
            var groupId = configuration.GroupId;
            if (String.IsNullOrEmpty(groupId))
            {
                return;
            }

            try
            {
                using var loading = dialog.Loading("Reset");
                using var client = new FaceClient(new ApiKeyServiceClientCredentials(configuration.ApiKey)) { Endpoint = configuration.ApiEndPoint };

                await client.PersonGroup.DeleteAsync(groupId);

                await dataService.DeleteAllPerson();
            }
            catch (Exception e)
            {
                await dialog.Information("Error", e.ToString());
            }
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "Ignore")]
        private async ValueTask<string> PrepareGroupAsync(FaceClient client, string recognitionModel)
        {
            var groupId = configuration.GroupId;
            if (String.IsNullOrEmpty(groupId))
            {
                groupId = Guid.NewGuid().ToString();
                configuration.GroupId = groupId;
            }

            try
            {
                await client.PersonGroup.CreateWithHttpMessagesAsync(groupId, groupId, recognitionModel: recognitionModel);
            }
            catch
            {
                // Do Nothing
            }

            return groupId;
        }
    }
}
