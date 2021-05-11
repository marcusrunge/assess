using Assessment.Models;
using Assessment.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace Assessment.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IFileService _fileService;
        private readonly IDicomService _dicomService;
        private string _title = "Assessment";
        private ICommand _menuFileOpenCommand, _menuFileSaveCommand, _menuFileSaveAsCommand;
        private Bitmap _dicomBitmap;
        private List<DicomMetaInfo> _dicomMetaInfos;

        public MainWindowViewModel(IFileService fileService, IDicomService dicomService)
        {
            _fileService = fileService;
            _dicomService = dicomService;
        }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        /// <summary>
        /// Gets or sets the DICOM bitmap
        /// </summary>
        public Bitmap DicomBitmap { get { return _dicomBitmap; } set { SetProperty(ref _dicomBitmap, value); } }

        /// <summary>
        /// Gets or sets the DICOM meta infos
        /// </summary>
        public List<DicomMetaInfo> DicomMetaInfos { get { return _dicomMetaInfos; } set { SetProperty(ref _dicomMetaInfos, value); } }

        /// <summary>
        /// Executes when file open menu item was selected
        /// </summary>
        public ICommand MenuFileOpenCommand => _menuFileOpenCommand ??= new DelegateCommand(async () =>
        {
            var path = _fileService.OpenFile();
            await _dicomService.ProcessFileAsync(path, (dicomMetaInfos, bitmap) =>
            {
                DicomBitmap = bitmap;
                DicomMetaInfos = dicomMetaInfos;
            });
        });

        /// <summary>
        /// Executes when file save menu item was selected
        /// </summary>
        public ICommand MenuFileSaveCommand => _menuFileSaveCommand ??= new DelegateCommand(() =>
        {

        });

        /// <summary>
        /// Executes when file save as menu item was selected
        /// </summary>
        public ICommand MenuFileSaveAsCommand => _menuFileSaveAsCommand ??= new DelegateCommand(async () =>
        {
            await _fileService.SaveImageFileAsync();
        });
    }
}
