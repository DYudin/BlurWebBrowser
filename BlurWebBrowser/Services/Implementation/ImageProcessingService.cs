using BlurWebBrowser.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BlurWebBrowser.Services.Implementation
{
    public class ImageProcessingService : IImageProcessingService
    {
        public Bitmap BlurImage(byte[] bytes, int blurLevel)
        {

            //todo:
            //return new byte{2,3 };
        }    
    }
}