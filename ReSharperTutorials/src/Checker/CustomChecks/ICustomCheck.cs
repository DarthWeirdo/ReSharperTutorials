using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.TextControl;

namespace ReSharperTutorials.Checker
{
    internal interface ICustomCheck
    {
        ISolution Solution { get; set; }
        IEditorManager EditorManager { get; set; }
        ITextControlManager TextControlManager { get; set; }
        DocumentManager DocumentManager { get; set; }
    }
}