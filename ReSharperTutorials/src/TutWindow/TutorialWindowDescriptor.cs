using JetBrains.Application;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.ToolWindowManagement;

namespace ReSharperTutorials.TutWindow
{
    [ToolWindowDescriptor(
        ProductNeutralId = "TutorialWindow",
        Text = "ReSharper Tutorial",
        Icon = typeof(JetBrains.Ide.Resources.IdeThemedIcons.TextDocument),
        Type = ToolWindowType.SingleInstance,
        VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Solution,        
        InitialDocking = ToolWindowInitialDocking.Right)
        ]


    public class TutorialWindowDescriptor : ToolWindowDescriptor
    {
//        public TutorialWindowDescriptor(IApplicationHost host, IWindowBranding branding) : base(host, branding)
//        {
//        }

        public TutorialWindowDescriptor(IApplicationHost host) : base(host)
        {
        }

        [Action("Tutorial Window", Id = 101)]
        public class ShowToolWindow : ActivateToolWindowActionHandler<TutorialWindowDescriptor>
        {
        }
    }
}
