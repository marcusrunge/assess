using AForge.Imaging.Filters;
using System.Drawing;

namespace Assessment.Services
{
    /// <summary>
    /// The image processing service contract.
    /// Specifies all methods for performing image processing operations.
    /// </summary>
    public interface IImageProcessingService
    {
        /// <summary>
        /// <c>DetectEdges</c> detects edges
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>The <see cref="Bitmap"/> with detected edges</returns>
        Bitmap DetectEdges(Bitmap bitmap);
    }
    public class ImageProcessingService : IImageProcessingService
    {
        public Bitmap DetectEdges(Bitmap bitmap)
        {
            Grayscale grayscale = new(0.2125, 0.7154, 0.0721);
            HomogenityEdgeDetector edgeDetector = new();
            Bitmap grayImage = grayscale.Apply((Bitmap)bitmap.Clone());
            edgeDetector.ApplyInPlace(grayImage);
            return grayImage;
        }
    }
}
