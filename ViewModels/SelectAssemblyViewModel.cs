using AddinManagerWpf.Entities;
using Autodesk.RevitAddIns;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

            string? newAssemblyPath = this.ChooseAssemblyFile();
            if (newAssemblyPath == null) return;

            try
            {
                string fullClassName = this.SelectedAddinElement.FullClassName;

                AddInFileManager.UpdateAssemblyPath(addIn, fullClassName, newAssemblyPath);
                AddInsManager.RefreshAvailableVersions();
                MessageBox.Show($"AddIn's {fullClassName} assembly path successfully.", "sucess", MessageBoxButton.OK);
                window.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update AddIn: {ex.Message}", "error", MessageBoxButton.OK);
            }
        }

        private string? ChooseAssemblyFile() => IOUtils.ChooseFile("DLL files (*.dll)|*.dll", Path.GetDirectoryName(addIn.FullName));

        private bool CanExecute() => this.SelectedAddinElement != null;

        [RelayCommand]
        public static void Cancel(Window window) => window.Close();
    }
}
