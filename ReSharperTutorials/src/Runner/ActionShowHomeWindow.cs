using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.Application;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.MenuGroups;
using JetBrains.UI.ToolWindowManagement;
using ReSharperTutorials.TutorialUI;

namespace ReSharperTutorials.Runner
{
    [Action("ActionShowHomeWindow", "Tutorials...", Id = 987654321)]
    public class ActionShowHomeWindow : IExecutableAction, IInsertLast<MainMenuFeaturesGroup>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var globalSettings = context.GetComponent<GlobalSettings>();
            if (globalSettings.TutorialWindowManager != null && globalSettings.TutorialWindowManager.WindowsExist())
            {
                globalSettings.TutorialWindowManager.ShowHomeWindow();
                return;
            }

            var solutionStateTracker = context.GetComponent<SolutionStateTracker>();
            var shellLocks = context.GetComponent<IShellLocks>();
            var environment = context.GetComponent<IUIApplication>();
            var actionManager = context.GetComponent<IActionManager>();
            var toolWindowManager = context.GetComponent<ToolWindowManager>();
            var toolWindowDescriptor = context.GetComponent<TutorialWindowDescriptor>();
            var windowsHookManager = context.GetComponent<IWindowsHookManager>();
            var colorThemeManager = context.GetComponent<IColorThemeManager>();
            var threading = context.GetComponent<IThreading>();

            globalSettings.TutorialWindowManager = new TutorialWindowManager(globalSettings.Lifetime,
                solutionStateTracker,
                globalSettings, shellLocks, toolWindowManager, toolWindowDescriptor, environment, actionManager,
                windowsHookManager,
                colorThemeManager, threading);

            globalSettings.TutorialWindowManager.ShowHomeWindow();
        }
    }
}