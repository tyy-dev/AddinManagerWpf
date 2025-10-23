using AddinManagerWpf.Entities;
using Autodesk.RevitAddIns;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace AddinManagerWpf.ViewModels
{
    public partial class SelectAssemblyViewModel(RevitAddInManifest addIn) : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AddInElement> _addInElements = new([
            .. addIn.AddInApplications.Select(app => new AddInElement(app)),
            .. addIn.AddInCommands.Select(cmd => new AddInElement(cmd))
        ]);

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private AddInElement? _selectedAddinElement;

        [RelayCommand(CanExecute = nameof(CanExecute))]
        public void Ok(Window window)
        {
            if (this.SelectedAddinElement == null) return;

            string? newAssemblyPath = ChooseAssemblyFile(Path.GetDirectoryName(addIn.FullName));
            if (newAssemblyPath == null) return;

            try
            {
                string fullClassName = this.SelectedAddinElement.FullClassName;

                AddInFileManager.UpdateAssemblyPath(addIn, fullClassName, newAssemblyPath);
                AddInsManager.RefreshAvailableVersions();
                MessageBox.Show($"AddIn's {fullClassName} assembly path successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update AddIn: {ex.Message}");
            }
        }

        private static string? ChooseAssemblyFile(string? initialDirectory)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "DLL files (*.dll)|*.dll",
                InitialDirectory = initialDirectory
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private bool CanExecute() => this.SelectedAddinElement != null;

        [RelayCommand]
        public static void Cancel(Window window) => window.Close();
    }
}
