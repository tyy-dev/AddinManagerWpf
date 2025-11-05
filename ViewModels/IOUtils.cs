using Microsoft.Win32;
using System.Diagnostics;

namespace AddinManagerWpf.ViewModels
{
    public static class IOUtils
    {
        public static string? ChooseFile(string filter = "DLL files (*.dll)|*.dll", string? initialDirectory = null)
        {
            OpenFileDialog dialog = new()
            {
                Filter = filter,
                InitialDirectory = initialDirectory
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static void Open(string path, bool selectInExplorer = false, bool openFile = false)
        {
            if (string.IsNullOrEmpty(path))
                return;

            // Opens Explorer and selects the file
            if (selectInExplorer)
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            // Opens the file with its default associated application
            else if (openFile)
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            // Opens the folder
            else
                Process.Start("explorer.exe", path);
        }
    }
}
