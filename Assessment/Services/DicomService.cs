using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Assessment.Services
{
    /// <summary>
    /// The dicom service contract.
    /// Specifies all methods for performing Fellow Oak DICOM operations.
    /// </summary>
    public interface IDicomService
    {
        /// <summary>
        /// <c>ProcessFileAsync</c> asynchronously processes the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task ProcessFileAsync(string path, Action<Dictionary<string, string>, Bitmap> callback);
    }

    public class DicomService : IDicomService
    {
        public DicomService() => ImageManager.SetImplementation(WinFormsImageManager.Instance);

        public async Task ProcessFileAsync(string path, Action<Dictionary<string, string>, Bitmap> callback)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var file = await DicomFile.OpenAsync(path);
                    var image = new DicomImage(path);
                    var bitmap = image.RenderImage().AsSharedBitmap();                    
                    var walker = new DicomDatasetWalker(file.FileMetaInfo);
                    walker.Walk(new DumpWalker((dictionary) => callback?.Invoke(dictionary, bitmap)));
                }
                catch { }
            }
        }

        private class DumpWalker : IDicomDatasetWalker
        {
            private Dictionary<string, string> _dictionary;
            private Action<Dictionary<string, string>> _onEndWalk;
            private int _level = 0;

            public int Level
            {
                get { return _level; }
                set
                {
                    _level = value;
                    Indent = string.Empty;
                    for (int i = 0; i < _level; i++)
                        Indent += "    ";
                }
            }

            private string Indent { get; set; }

            public DumpWalker(Action<Dictionary<string, string>> onEndWalk)
            {
                _onEndWalk = onEndWalk;
                _dictionary = new Dictionary<string, string>();
            }

            public bool OnBeginFragment(DicomFragmentSequence fragment)
            {
                var tag = string.Format("{0}{1}  {2}", Indent, fragment.Tag.ToString().ToUpper(), fragment.Tag.DictionaryEntry.Name);
                _dictionary.Add(tag, string.Empty);
                Level++;
                return true;
            }

            public bool OnBeginSequence(DicomSequence sequence)
            {
                var tag = string.Format("{0}{1}  {2}", Indent, sequence.Tag.ToString().ToUpper(), sequence.Tag.DictionaryEntry.Name);
                _dictionary.Add(tag, string.Empty);
                Level++;
                return true;
            }

            public bool OnBeginSequenceItem(DicomDataset dataset)
            {
                var tag = string.Format("{0}Sequence Item:", Indent);
                _dictionary.Add(tag, string.Empty);
                Level++;
                return true;
            }

            public void OnBeginWalk() { }

            public bool OnElement(DicomElement element)
            {
                var tag = string.Format("{0}{1}  {2}", Indent, element.Tag.ToString().ToUpper(), element.Tag.DictionaryEntry.Name);
                string value = "<large value not displayed>";
                if (element.Length <= 2048) value = string.Join("\\", element.Get<string[]>());
                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    var uid = element.Get<DicomUID>(0);
                    var name = uid.Name;
                    if (name != "Unknown") value = string.Format("{0} ({1})", value, name);
                }
                _dictionary.Add(tag, value);
                return true;
            }

            public Task<bool> OnElementAsync(DicomElement element)
            {
                string value = "<large value not displayed>";
                if (element.Length <= 2048) value = string.Join("\\", element.Get<string[]>());
                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    var uid = element.Get<DicomUID>(0);
                    var name = uid.Name;
                    if (name != "Unknown") value = string.Format("{0} ({1})", value, name);
                }
                _dictionary.Add(element.Tag.DictionaryEntry.Name, value);
                return Task.FromResult(true);
            }

            public bool OnEndFragment()
            {
                Level--;
                return true;
            }

            public bool OnEndSequence()
            {
                Level--;
                return true;
            }

            public bool OnEndSequenceItem()
            {
                Level--;
                return true;
            }

            public void OnEndWalk() => _onEndWalk?.Invoke(_dictionary);

            public bool OnFragmentItem(IByteBuffer item)
            {
                var tag = string.Format("{0}Fragment", Indent);
                _dictionary.Add(tag, string.Empty);
                return true;
            }

            public Task<bool> OnFragmentItemAsync(IByteBuffer item) => Task.FromResult(true);
        }
    }
}
