using AddinManagerWpf.Entities;
using AddinManagerWpf.Windows;
using Autodesk.RevitAddIns;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace AddinManagerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<RevitVersionInfo> _versions = new(AddInsManager.AvailableVersions);

        [RelayCommand]
        public void RefreshAddIns()
        {
            AddInsManager.RefreshAvailableVersions();
            this.Versions = new(AddInsManager.AvailableVersions);
        }

        [RelayCommand]
        public static void OpenAddInFolder(RevitAddInManifest addIn) => Process.Start("explorer.exe", $"/select,\"{addIn.FullName}\"");

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
    }
}
