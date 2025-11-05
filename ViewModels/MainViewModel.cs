using AddinManagerWpf.Entities;
using AddinManagerWpf.Windows;
using Autodesk.RevitAddIns;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace AddinManagerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<RevitVersionInfo> _versions = new(AddInsManager.AvailableVersions);

        [ObservableProperty]
        private ObservableCollection<string> _basePaths = new(AddInsManager.AvailableBasePaths);

        [ObservableProperty]
        private string _openBasePathSelectedPath = string.Empty;

        #region Commands
        [RelayCommand]
        public void OpenBasePathFolder(string basePath) => IOUtils.Open(basePath);

        [RelayCommand]
        public void RefreshAddIns()
        {
            AddInsManager.RefreshAvailableVersions();
            this.Versions = new(AddInsManager.AvailableVersions);
            this.BasePaths = new(AddInsManager.AvailableBasePaths);
        }

        [RelayCommand]
        public static void OpenAddInFolder(RevitAddInManifest addIn) => IOUtils.Open(addIn.FullName, selectInExplorer: true);

        [RelayCommand]
        public static void OpenAddIn(RevitAddInManifest addIn) => IOUtils.Open(addIn.FullName, openFile: true);
        [RelayCommand]
        public static void SwitchAssembly(RevitAddInManifest addIn)
        {
            SelectAssemblyWindow window = new()
            {
                DataContext = new SelectAssemblyViewModel(addIn),
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }
        #endregion
    }
}
