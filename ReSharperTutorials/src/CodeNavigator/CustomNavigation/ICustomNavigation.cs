using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;

namespace ReSharperTutorials.CodeNavigator
{
    interface ICustomNavigation
    {
        ISolution Solution { get; set; }
        IEditorManager EditorManager { get; set; }
        DocumentManager DocumentManager { get; set; }
    }
}
