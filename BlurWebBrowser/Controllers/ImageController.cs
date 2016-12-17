using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BlurWebBrowser.ViewModels;
using System.Threading.Tasks;
using BlurWebBrowser.Services.Abstract;

namespace BlurWebBrowser.Controllers
{
    public class ImageController : ApiController
    {
        private readonly IImageEngine _imageEngine;

        public ImageController(
            IImageEngine imageEngine)
        {
            if (imageEngine == null) throw new ArgumentNullException(("imageEngine"));

            _imageEngine = imageEngine;      
        }

        [Route("api/image/blurimages")]
        [HttpPost]
        public async Task<IHttpActionResult> BlurImages(IEnumerable<ImageViewModel> imageVMs)
        {
            List<ImageViewModel> processedImages = new List<ImageViewModel>();

            try
            {
                foreach (var imageVM in imageVMs)
                {
                    var processedImage = await blurImageInternal(imageVM);
                    processedImages.Add(processedImage);
                }
            } 
            catch
            {

            }
            finally
            {
                // TODO
            }

            return Ok(processedImages);
        }

        private Task<ImageViewModel> blurImageInternal(ImageViewModel imageVM)
        {
            var processedImageVM = new ImageViewModel();

            return Task.Run(() =>
            {
                // call services
                processedImageVM.Data = _imageEngine.BlurImage(imageVM.Data).ToArray();
              
                return processedImageVM;
            });      
        }
    }
}
