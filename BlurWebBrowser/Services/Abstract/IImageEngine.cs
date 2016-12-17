
using System.Collections.Generic;

namespace BlurWebBrowser.Services.Abstract
{
    public interface IImageEngine
    {
        IEnumerable<byte> BlurImage(IEnumerable<byte> inputImage);
    }
}