using Autodesk.RevitAddIns;
using System.Text.RegularExpressions;

namespace AddinManagerWpf.Entities
{
    public partial record RevitVersionInfo(string Version)
    {
        [GeneratedRegex(@"^20\d{2}$")]
        private static partial Regex ValidRevitVersionRegex();
        public List<RevitAddInManifest> AddIns { get; init; } = [];
        public bool ValidateVersion() => ValidRevitVersionRegex().IsMatch(this.Version);
    }
}
