using Autodesk.RevitAddIns;
using System.IO;

namespace AddinManagerWpf
{
    public static class AddInLoader
    {
        public static RevitAddInManifest? TryLoadAddIn(string addInPath)
        {
            try
            {
                return AddInManifestUtility.GetRevitAddInManifest(addInPath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<RevitAddInManifest> LoadAddInsFromDirectory(string path) =>
            [.. Directory
                .EnumerateFiles(path, "*.addin")
                .Select(TryLoadAddIn)
                .Where(a => a is not null).Cast<RevitAddInManifest>()];
    }
}
