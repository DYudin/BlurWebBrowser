using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlurWebBrowser.Services.Abstract
{
    public interface IImageEngine
    {
        IEnumerable<byte> BlurImage(IEnumerable<byte> inputImage);
    }
}