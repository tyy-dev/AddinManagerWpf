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

        private string? ChooseAssemblyFile() => IOUtils.ChooseFile(initialDirectory: Path.GetDirectoryName(addIn.FullName));

        private bool CanExecute() => this.SelectedAddinElement != null;

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecute))]
        public void Ok(Window window)
        {
            if (this.SelectedAddinElement == null) return;

            string? newAssemblyPath = this.ChooseAssemblyFile();
            if (newAssemblyPath == null) return;

            string fullClassName = this.SelectedAddinElement.FullClassName;
            try
            {
                AddInFileManager.UpdateAssemblyPath(addIn, fullClassName, newAssemblyPath);
                AddInsManager.RefreshAvailableVersions();
                MessageBox.Show($"AddIn's {fullClassName} assembly path successfully.", "sucess", MessageBoxButton.OK);
                window.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update AddIn {fullClassName}: {ex.Message}", "error", MessageBoxButton.OK);
            }
        }
        [RelayCommand]
        public static void Cancel(Window window) => window.Close();
        #endregion
    }
}
