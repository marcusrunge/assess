using Assessment.Services;
using Dicom;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;

namespace Assessment.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IFileService _fileService;
        private readonly IDicomService _dicomService;
        private string _title = "Assessment";
        private ICommand _menuFileOpenCommand, _menuFileSaveCommand, _menuFileSaveAsCommand;
        private Bitmap _dicomBitmap;
        private Dictionary<string, string> _dictionary;

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
        /// Gets or sets the record
        /// </summary>
        public Dictionary<string, string> Dictionary { get { return _dictionary; } set { SetProperty(ref _dictionary, value); } }

        /// <summary>
        /// Executes when file open menu item was selected
        /// </summary>
        public ICommand MenuFileOpenCommand => _menuFileOpenCommand ??= new DelegateCommand(async () =>
        {
            var path = _fileService.OpenFile();
            await _dicomService.ProcessFileAsync(path, (dictionary, bitmap) =>
            {
                DicomBitmap = bitmap;
                Dictionary = dictionary;
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
        public ICommand MenuFileSaveAsCommand => _menuFileSaveAsCommand ??= new DelegateCommand(() =>
        {
            _fileService.SaveImageFile();
        });
    }
}
