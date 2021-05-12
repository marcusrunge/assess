using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assessment.Services
{
    /// <summary>
    /// The file service contract.
    /// Specifies all methods for performing file operations.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// <c>OpenFile</c> opens any file
        /// </summary>
        /// <returns>The file path</returns>
        string OpenFile();
        /// <summary>
        /// <c>OpenFolder</c> opens any folder
        /// </summary>
        /// <returns>The folder path</returns>
        string OpenFolder();
        /// <summary>
        /// <c>SaveImageFile</c> saves an image to file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The completed task</returns>
        Task SaveImageFileAsync(string fileName = null);
    }

    /// <summary>
    /// The file service class.
    /// Contains all methods for performing file operations.
    /// </summary>
    public class FileService : IFileService
    {
        private IDicomService _dicomService;

        public FileService(IDicomService dicomService) => _dicomService = dicomService;

        public string OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();
            var result = openFileDialog.ShowDialog();
            return result.HasValue && result.Value ? openFileDialog.FileName : null;
        }

        public string OpenFolder()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            return result.HasFlag(DialogResult.OK) ? folderBrowserDialog.SelectedPath : null;
        }

        public async Task SaveImageFileAsync(string fileName)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new();
            saveFileDialog.FileName = fileName;
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.Filter = "BMP (*.bmp)|*.bmp|PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
            var result = saveFileDialog.ShowDialog();
            if (result.Value)
            {
                var extension = Path.GetExtension(saveFileDialog.FileName);
                switch (extension.ToLower())
                {
                    case ".bmp":
                        await _dicomService.ExportDicomFileAsync(saveFileDialog.FileName, ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        await _dicomService.ExportDicomFileAsync(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case ".png":
                        await _dicomService.ExportDicomFileAsync(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(extension);
                }
            }
        }
    }
}
