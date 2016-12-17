
using System.Collections.Generic;
using System.Drawing;

namespace BlurWebBrowser.Services.Abstract
{
    public interface IImageRedactor
    {
        Bitmap BlurImage(IEnumerable<byte> inputImage, int blurLevel);            
    }
}