using BlurWebBrowser.Services.Abstract;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace BlurWebBrowser.Services.Implementation
{
    public class GoogleApiWrapper
    {
        private VisionService _visionService;
        private readonly IImageProcessingService _imageService;
        public GoogleApiWrapper(IImageProcessingService imageService)
        {
            if (imageService == null) throw new ArgumentNullException(("imageService"));
            _imageService = imageService;
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

        public IEnumerable<byte> BlurImages(IEnumerable<byte> inputImage)
        {
            int MAX_RESULTS = 90;

            IEnumerable<FaceAnnotation> faces = detectFaces(inputImage, MAX_RESULTS);
            processFaces(inputImage, faces);

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

        private void processFaces(IEnumerable<byte> inputImage, IEnumerable<FaceAnnotation> faces)
        {
            Bitmap img = new Bitmap(inputPath);          

            foreach (FaceAnnotation face in faces)
            {
                // 1.Get face location
                var rect = createRectangelAroundFace(img, face);

                // 2. Create image with face
                System.Drawing.Image bmp = new Bitmap(rect.Width + 40, rect.Height + 50);
                                               

                // 3. Blur
                ImageConverter converter = new ImageConverter();
                var bytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                Bitmap bitMap = _imageService.BlurImage(bytes, 35);

                //Effects.DrawPoly(img, face);

                // 4. Insert blurred face image into source image
                Graphics gr = Graphics.FromImage(img);
                gr.DrawImage(bitMap, new Point(face.FdBoundingPoly.Vertices[0].X.Value, face.FdBoundingPoly.Vertices[0].Y.Value));

            }
        }

        private static Rectangle createRectangelAroundFace(Bitmap img, FaceAnnotation face)
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