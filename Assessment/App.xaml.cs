using Assessment.Services;
using Assessment.Views;
using Prism.Ioc;
using System.Windows;

namespace Assessment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileService, FileService>();
            containerRegistry.Register<IDicomService, DicomService>();
        }
    }
}
