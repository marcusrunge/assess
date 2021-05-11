using Assessment.Models;
using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

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
        Task ProcessFileAsync(string path, Action<List<DicomMetaInfo>, Bitmap> callback);
    }

    public class DicomService : IDicomService
    {
        public DicomService() => ImageManager.SetImplementation(WinFormsImageManager.Instance);

        public async Task ProcessFileAsync(string path, Action<List<DicomMetaInfo>, Bitmap> callback)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var file = await DicomFile.OpenAsync(path);
                    var image = new DicomImage(path);
                    var bitmap = image.RenderImage().AsSharedBitmap();
                    var walker = new DicomDatasetWalker(file.Dataset);
                    walker.Walk(new DumpWalker((dicomMetaInfos) => callback?.Invoke(dicomMetaInfos, bitmap)));
                }
                catch { }
            }
        }

        private class DumpWalker : IDicomDatasetWalker
        {
            private Action<List<DicomMetaInfo>> _onEndWalk;
            private List<DicomMetaInfo> _dicomMetaInfos;
            private int _level = 0;

            public int Level
            {
                get { return _level; }
                set
                {
                    _level = value;
                    Indent = string.Empty;
                    for (int i = 0; i < _level; i++) Indent += "    ";
                }
            }

            private string Indent { get; set; }

            public DumpWalker(Action<List<DicomMetaInfo>> onEndWalk)
            {
                _onEndWalk = onEndWalk;
                _dicomMetaInfos = new List<DicomMetaInfo>();
            }

            public bool OnBeginFragment(DicomFragmentSequence fragment)
            {
                var tag = string.Format("{0}{1}  {2}", Indent, fragment.Tag.ToString().ToUpper(), fragment.Tag.DictionaryEntry.Name);
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = fragment.ValueRepresentation.Code, Length = string.Empty, Value = string.Empty });
                Level++;
                return true;
            }

            public bool OnBeginSequence(DicomSequence sequence)
            {
                var tag = string.Format("{0}{1}  {2}", Indent, sequence.Tag.ToString().ToUpper(), sequence.Tag.DictionaryEntry.Name);
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = "SQ", Length = string.Empty, Value = string.Empty });
                Level++;
                return true;
            }

            public bool OnBeginSequenceItem(DicomDataset dataset)
            {
                var tag = string.Format("{0}Sequence Item:", Indent);
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = string.Empty, Length = string.Empty, Value = string.Empty });
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
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = element.ValueRepresentation.Code, Length = element.Length.ToString(), Value = value });
                return true;
            }

            public Task<bool> OnElementAsync(DicomElement element)
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
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = element.ValueRepresentation.Code, Length = element.Length.ToString(), Value = value });
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

            public void OnEndWalk() => _onEndWalk?.Invoke(_dicomMetaInfos);

            public bool OnFragmentItem(IByteBuffer item)
            {
                var tag = string.Format("{0}Fragment", Indent);
                _dicomMetaInfos.Add(new DicomMetaInfo { Tag = tag, Code = string.Empty, Length = item.Size.ToString(), Value = string.Empty });
                return true;
            }

            public Task<bool> OnFragmentItemAsync(IByteBuffer item) => Task.FromResult(true);
        }
    }
}
