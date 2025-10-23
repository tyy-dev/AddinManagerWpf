using Autodesk.RevitAddIns;
using System;
using System.IO;
using System.Xml.Linq;

namespace AddinManagerWpf
{
    public static class AddInFileManager
    {
        public static void UpdateAssemblyPath(RevitAddInManifest addIn, string fullClassName, string newAssemblyPath)
        {
            string addInPath = addIn.FullName;

            if (!File.Exists(addInPath))
                throw new FileNotFoundException("AddIn file not found", addInPath);

            XDocument doc = XDocument.Load(addInPath);

            XElement? addInElement = doc.Root?
                .Elements("AddIn")
                .FirstOrDefault(e => string.Equals(
                    e.Element("FullClassName")?.Value,
                    fullClassName,
                    StringComparison.OrdinalIgnoreCase));

            if (addInElement == null)
                throw new InvalidOperationException($"AddIn with FullClassName '{fullClassName}' not found in {addInPath}.");

            XElement? assemblyElement = addInElement.Element("Assembly");
            assemblyElement?.Value = newAssemblyPath;

            doc.Save(addInPath);
        }
    }
}