using BlurWebBrowser.Services.Abstract;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BlurWebBrowser.Services.Implementation
{
    public class GoogleApiWrapper : IImageEngine
    {
        private VisionService _visionService;
        private readonly IImageRedactor _imageRedactor;
        public GoogleApiWrapper(IImageRedactor imageRedactor)
        {
            if (imageRedactor == null) throw new ArgumentNullException(("imageRedactor"));
            _imageRedactor = imageRedactor;
            Configure();
        }

        private void Configure()
        {
            _visionService = new VisionService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyDbuBnG-8f41OVET1BXXoHjhZRTlQFFnvU",
                GZipEnabled = false

            });
        }

        public IEnumerable<byte> BlurImage(IEnumerable<byte> inputImageBytes)
        {
            if (inputImageBytes == null) throw new ArgumentNullException(("inputImageBytes"));

            IEnumerable<byte> outputImageBytes = new List<byte>();

            int MAX_RESULTS = 100;
            int blurLevel = 30;

            
            using (var ms = new MemoryStream(inputImageBytes.ToArray()))
            {
                using (Bitmap sourceImage = new Bitmap(ms))
                {
                    // Detect all faces on input images with google api
                    IEnumerable<FaceAnnotation> faces = detectFaces(inputImageBytes, MAX_RESULTS);

                    // Blur all faces images
                    IEnumerable<FaceImage> bluredFacesImages = blurFaces(sourceImage, faces, blurLevel);

                    // Combine source image with blurred faces images
                    outputImageBytes = combine(sourceImage, bluredFacesImages);
                }                    
            }         

            return outputImageBytes;
        }

        IEnumerable<FaceAnnotation> detectFaces(IEnumerable<byte> inputImage, int maxResults)
        {      
            var img = new Google.Apis.Vision.v1.Data.Image();
            img.Content = Convert.ToBase64String(inputImage.ToArray());

            AnnotateImageRequest request = new AnnotateImageRequest();
            request.Image = img;
            request.Features = new List<Feature>();
            request.Features.Add(new Feature() { MaxResults = maxResults, Type = "FACE_DETECTION" });

            var batch = new BatchAnnotateImagesRequest();
            batch.Requests = new List<AnnotateImageRequest>();
            batch.Requests.Add(request);
            var annotate = _visionService.Images.Annotate(batch);
            //annotate.Key = "AIzaSyDbuBnG-8f41OVET1BXXoHjhZRTlQFFnvU";

            BatchAnnotateImagesResponse batchResponse = annotate.Execute();

            AnnotateImageResponse response = batchResponse.Responses[0];
            if (response.FaceAnnotations == null)
            {
                throw new IOException(
                    response.Error != null
                        ? response.Error.Message
                        : "Unknown error getting image annotations");
            }

            return response.FaceAnnotations;
        }

        private IEnumerable<FaceImage> blurFaces(Bitmap inputImage, IEnumerable<FaceAnnotation> faces, int blurLevel)
        {
            var result = new List<FaceImage>();          

            foreach (FaceAnnotation face in faces)
            {    
                var faceImage = new FaceImage()
                { Position = new Point(face.FdBoundingPoly.Vertices[0].X.Value, face.FdBoundingPoly.Vertices[0].Y.Value) };
                // 1. Create rectangl with face location
                var rect = createRectangleAroundFace(face);

                // 2. Create image with face               
                using (System.Drawing.Image bmp = new Bitmap(rect.Width, rect.Height))
                {
                    using (Graphics gr = Graphics.FromImage(bmp))
                    {
                        gr.DrawImage(inputImage, 0, 0, rect, GraphicsUnit.Pixel);
                    }

                    var converter = new ImageConverter();
                    var bytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                    // 3. Blur face image
                    faceImage.Content = _imageRedactor.BlurImage(bytes, blurLevel);
                    result.Add(faceImage);
                }
            }

            return result;
        }

        private IEnumerable<byte> combine(Bitmap inputImage, IEnumerable<FaceImage> blurredFaces)
        {
            IEnumerable<byte> result = new List<byte>();

            using (Graphics gr = Graphics.FromImage(inputImage))
            {
                foreach (var blurredFace in blurredFaces)
                {
                    gr.DrawImage(blurredFace.Content, blurredFace.Position);
                }
            }           
          
            var converter = new ImageConverter();
            // 4.3 Convert to bytes
            result = (byte[])converter.ConvertTo(inputImage, typeof(byte[]));           

            return result;
        }

        private Rectangle createRectangleAroundFace(FaceAnnotation face)
        {
            Rectangle rectangle;           

            var firstPointX = face.FdBoundingPoly.Vertices[0].X;
            var firstPointY = face.FdBoundingPoly.Vertices[0].Y;
            var width = face.FdBoundingPoly.Vertices[1].X - face.FdBoundingPoly.Vertices[0].X;
            var height = face.FdBoundingPoly.Vertices[2].Y - face.FdBoundingPoly.Vertices[1].Y;
            rectangle = new Rectangle(firstPointX.Value, firstPointY.Value, width.Value, height.Value);

            return rectangle;
        }
    }
}