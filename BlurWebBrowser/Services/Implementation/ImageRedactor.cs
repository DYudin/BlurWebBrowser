using BlurWebBrowser.Services.Abstract;
using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BlurWebBrowser.Services.Implementation
{
    public class ImageRedactor : IImageRedactor
    {
        public Bitmap BlurImage(IEnumerable<byte> inputImage, int blurLevel)
        {
            if (inputImage == null) throw new ArgumentNullException(("inputImage"));

            Bitmap bitMap;

            using (MemoryStream inStream = new MemoryStream(inputImage.ToArray()))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory())
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .GaussianBlur(blurLevel)
                            .Save(outStream);
                    }

                    bitMap = new Bitmap(outStream);
                }
            }

            return bitMap;
        }    
    }
}