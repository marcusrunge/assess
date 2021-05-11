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
    }

    /// <summary>
    /// The file service class.
    /// Contains all methods for performing file operations.
    /// </summary>
    public class FileService : IFileService
    {
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
    }
}
