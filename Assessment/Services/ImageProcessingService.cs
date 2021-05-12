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

        /// <summary>
        /// <c>CorrectContrast</c> corrects contrast
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>The <see cref="Bitmap"/> with corrected contrast</returns>
        Bitmap CorrectContrast(Bitmap bitmap);
    }
    public class ImageProcessingService : IImageProcessingService
    {
        public Bitmap CorrectContrast(Bitmap bitmap)
        {
            ContrastCorrection contrastCorrection = new(50);
            var correctedBitmap = contrastCorrection.Apply((Bitmap)bitmap.Clone());
            return correctedBitmap;
        }

        public Bitmap DetectEdges(Bitmap bitmap)
        {
            Grayscale grayscale = new(0.2125, 0.7154, 0.0721);
            HomogenityEdgeDetector edgeDetector = new();
            Bitmap grayBitmap = grayscale.Apply((Bitmap)bitmap.Clone());
            edgeDetector.ApplyInPlace(grayBitmap);
            return grayBitmap;
        }
    }
}
