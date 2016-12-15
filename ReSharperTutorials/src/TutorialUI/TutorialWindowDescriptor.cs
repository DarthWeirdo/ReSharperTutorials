using JetBrains.Application;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.ToolWindowManagement;

namespace ReSharperTutorials.TutorialUI
{
    [ToolWindowDescriptor(
            ProductNeutralId = "TutorialWindow",
            Text = "ReSharper Tutorials",
            Icon = typeof(JetBrains.Ide.Resources.IdeThemedIcons.TextDocument),
            Type = ToolWindowType.MultiInstance,
            VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Global,
            InitialDocking = ToolWindowInitialDocking.Right)
    ]
    public class TutorialWindowDescriptor : ToolWindowDescriptor
    {
        public TutorialWindowDescriptor(IApplicationHost host) : base(host)
        {
        }

        [Action("Tutorial Window", Id = 87654324)]
        public class ShowToolWindow : ActivateToolWindowActionHandler<TutorialWindowDescriptor>
        {
        }
    }
}