using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Assessment.Services
{
    /// <summary>
    /// The image processing service contract.
    /// Specifies all methods for performing image processing operations.
    /// </summary>
    public interface IImageProcessingService
    {
        /// <summary>
        /// <c>DetectEdgesAsync</c> asynchronously detects edges
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>The <see cref="Bitmap"/> with detected edges</returns>
        Task<Bitmap> DetectEdgesAsync(Bitmap bitmap);
    }
    public class ImageProcessingService : IImageProcessingService
    {
        public Task<Bitmap> DetectEdgesAsync(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }
    }
}
