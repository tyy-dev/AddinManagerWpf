using Autodesk.RevitAddIns;

namespace AddinManagerWpf.Entities
{
    public class AddInElement
    {
        public string DisplayName { get; set; }
        public string FullClassName { get; set; }

        public AddInElement(object element)
        {
            (this.FullClassName, this.DisplayName) = element switch
            {
                RevitAddInApplication app => (app.FullClassName, $"[Application] {app.FullClassName}"),
                RevitAddInCommand cmd => (cmd.FullClassName, $"[Command] {cmd.FullClassName}"),
                _ => ("Unknown", "Unknown")
            };
        }
    }
}
