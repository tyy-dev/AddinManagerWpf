using AddinManagerWpf.Entities;
using System.Collections.ObjectModel;
using System.IO;
namespace AddinManagerWpf
{
    public static class AddInsManager
    {
        public static ObservableCollection<RevitVersionInfo> AvailableVersions { get; private set; } = [];

        public static List<string> RevitAddInBasePaths { get; set; } = [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Autodesk", "Revit", "Addins"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Autodesk", "ApplicationPlugins"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Autodesk", "Revit", "Addins")
        ];

        public static void RefreshAvailableVersions()
        {
            AvailableVersions.Clear();

            // Track versions we've already added, so there aren't duplicate "version" entries and addins are merged.
            Dictionary<string, RevitVersionInfo> versionDict = [];

            foreach (string basePath in RevitAddInBasePaths)
            {
                if (!Directory.Exists(basePath)) continue;

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

                    AvailableVersions.Add(revitVersionInfo);
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
