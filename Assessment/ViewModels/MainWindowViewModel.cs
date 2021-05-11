using Assessment.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace Assessment.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IFileService _fileService;
        private string _title = "Assessment";
        private ICommand _menuFileOpenCommand, _menuFileSaveCommand, _menuFileSaveAsCommand;
          
        public MainWindowViewModel(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        /// <summary>
        /// Executes when file open menu item was selected
        /// </summary>
        public ICommand MenuFileOpenCommand => _menuFileOpenCommand ??= new DelegateCommand(() =>
        {
            var path = _fileService.OpenFile();
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

        });
    }
}
