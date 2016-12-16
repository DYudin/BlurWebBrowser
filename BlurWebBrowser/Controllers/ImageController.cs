using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlurWebBrowser.ViewModels;
using System.Threading.Tasks;

namespace BlurWebBrowser.Controllers
{
    public class ImageController : ApiController
    {
        private readonly IGoogleApiWrapper _wrapper;

        public TransactionController(
            IGoogleApiWrapper wrapper)
        {
            if (wrapper == null) throw new ArgumentNullException(("wrapper"));
            
            _wrapper = wrapper;      
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
            finally
            {
                // TODO
            }

            return Ok(processedImages);
        }

        private Task<ImageViewModel> blurImageInternal(ImageViewModel imageVM)
        {
            return Task.Run(() =>
            {
                // call services



                return new ImageViewModel();

            });      
        }
    }
}
