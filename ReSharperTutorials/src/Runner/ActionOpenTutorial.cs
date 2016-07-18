using System;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.DataFlow;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.Application;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.MenuGroups;
using JetBrains.UI.ToolWindowManagement;
using JetBrains.Util;
using ReSharperTutorials.TutorialUI;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{    
    public abstract class ActionOpenTutorial : IExecutableAction
    {

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            OpenTutorial(context, nextExecute);
        }

        protected abstract void OpenTutorial(IDataContext context, DelegateExecute nextExecute);

        protected static void OpenOrRestart(IDataContext context, TutorialId id)
        {
            var globalOptions = context.GetComponent<GlobalSettings>();
            var titleString = TutorialXmlReader.ReadIntro(globalOptions.GetPath(id, PathType.WorkCopyContentFile));
            var step = TutorialXmlReader.ReadCurrentStep(globalOptions.GetPath(id, PathType.WorkCopyContentFile));
            var firstTime = step == 1;
                
            VsIntegration.CloseVsSolution();
            SolutionCopyHelper.CopySolution(globalOptions.GetPath(id, PathType.BaseSolutionFolder),
                globalOptions.GetPath(id, PathType.WorkCopySolutionFolder));
            VsIntegration.OpenVsSolution(globalOptions.GetPath(id, PathType.WorkCopySolutionFile));                
        }

    }


    [Action("ActionOpenTutorial1", "Start Tutorial 1 - Essential Shortcuts", Id = 100)]
    public class ActionOpenTutorial1 : ActionOpenTutorial
    {
        protected override void OpenTutorial(IDataContext context, DelegateExecute nextExecute)
        {
            OpenOrRestart(context, TutorialId.Tutorial1);
        }
    }

    [Action("ActionShowMainTutorialWindow", "ReSharper Tutorials...", Id = 121)]
    public class ActionShowMainTutorialWindow : ActionOpenTutorial, IInsertLast<MainMenuFeaturesGroup>
    {
        protected override void OpenTutorial(IDataContext context, DelegateExecute nextExecute)
        {
            var globalSettings = context.GetComponent<GlobalSettings>();
            if (globalSettings.TutorialWindowManager != null && globalSettings.TutorialWindowManager.WindowsExist())
            {
                globalSettings.TutorialWindowManager.ShowHomeWindow();
                return;
            }                
            
            var shellLocks = context.GetComponent<IShellLocks>();
            var environment = context.GetComponent<IUIApplication>();
            var actionManager = context.GetComponent<IActionManager>();
            var toolWindowManager = context.GetComponent<ToolWindowManager>();
            var toolWindowDescriptor = context.GetComponent<TutorialWindowDescriptor>();
            var windowsHookManager = context.GetComponent<IWindowsHookManager>();
            var colorThemeManager = context.GetComponent<IColorThemeManager>();
            var threading = context.GetComponent<IThreading>();

            globalSettings.TutorialWindowManager = new TutorialWindowManager(globalSettings.Lifetime, globalSettings, shellLocks, 
                toolWindowManager, toolWindowDescriptor, environment, actionManager, windowsHookManager, colorThemeManager, threading);

            globalSettings.TutorialWindowManager.ShowHomeWindow();
        }
    }
}