using AddinManagerWpf.Entities;
using System.Collections.ObjectModel;
using System.IO;
namespace AddinManagerWpf
{
    public static class AddInsManager
    {
        public static ObservableCollection<RevitVersionInfo> AvailableVersions { get; private set; } = [];

        /// <summary>
        /// Only directories that exist
        /// </summary>
        public static List<string> AvailableBasePaths { get; private set; } = [];

        private static readonly List<string> PossibleBasePaths =
        [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Autodesk", "Revit", "Addins"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Autodesk", "ApplicationPlugins"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Autodesk", "Revit", "Addins"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Autodesk", "ApplicationPlugins")
        ];

        public static List<string> RefreshBasePaths()
        {
            AvailableBasePaths = [.. PossibleBasePaths.Where(Directory.Exists)];
            return AvailableBasePaths;
        }
        /// <summary>
        /// Refreshes the collection of available Base Paths & Revit versions and their associated add-ins
        /// </summary>
        public static void RefreshAvailableVersions()
        {
            AvailableVersions.Clear();

            // Track versions we've already added, so there aren't duplicate "version" entries and addins are merged.
            Dictionary<string, RevitVersionInfo> versionDict = [];

            List<string> basePaths = RefreshBasePaths();
            foreach (string basePath in basePaths)
            {
                foreach (string versionDir in Directory.EnumerateDirectories(basePath))
                {
                    string version = Path.GetFileName(versionDir);

                    if (!versionDict.TryGetValue(version, out RevitVersionInfo? revitVersionInfo))
                    {
                        revitVersionInfo = new(version);

                        // Skip non-version folders
                        if (!revitVersionInfo.ValidateVersion())
                            continue;

                        versionDict[version] = revitVersionInfo;
                    }

                    revitVersionInfo.AddIns.AddRange(AddInLoader.LoadAddInsFromDirectory(versionDir));
                }
            }

            // Only load versions that have addins
            AvailableVersions = new(versionDict.Values.Where(revitVersionInfo => revitVersionInfo.AddIns.Count != 0));

            RevitVersionInfo allVersion = new("ALL")
            {
                AddIns = [.. AvailableVersions.SelectMany(v => v.AddIns)]
            };

            // Add "ALL" first
            AvailableVersions.Insert(0, allVersion);
        }

        static AddInsManager() => RefreshAvailableVersions();
    }
}
