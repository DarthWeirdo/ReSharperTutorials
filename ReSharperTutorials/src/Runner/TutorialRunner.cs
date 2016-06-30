using System;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised.Shortcuts;
using JetBrains.UI.Application;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.ToolWindowManagement;
using ReSharperTutorials.TutWindow;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{       
    [SolutionComponent]
    public class TutorialRunner
    {
        public TutorialRunner([NotNull] Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
                                  [NotNull] ISolutionStateTracker solutionStateTracker,
                                  [NotNull] GlobalSettings globalSettings, TextControlManager textControlManager, IShellLocks shellLocks,
                                  IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment, 
                                  IActionManager actionManager, ToolWindowManager toolWindowManager, TutorialWindowDescriptor tutorialWindowDescriptor,
                                  IWindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
                                  IColorThemeManager colorThemeManager, IThreading threading)
        {
            if (lifetime == null)
                throw new ArgumentNullException("lifetime");
            if (solutionStateTracker == null)
                throw new ArgumentNullException("solutionStateTracker");
            if (globalSettings == null)
                throw new ArgumentNullException("globalSettings");
                        

            foreach (var tutorial in globalSettings.AvailableTutorials)
            {
                if (VsCommunication.GetCurrentSolutionPath() == tutorial.Value)
                {
                    solutionStateTracker.AfterPsiLoaded.Advise(lifetime, 
                    sol => RunTutorial(globalSettings.GetPath(tutorial.Key, PathType.WorkCopyContentFile), lifetime, solution, psiFiles, 
                        textControlManager, shellLocks, editorManager, documentManager, environment, actionManager, toolWindowManager, 
                        tutorialWindowDescriptor, windowsHookManager, psiServices, shortcutManager, colorThemeManager, threading));                    
                }
            }                                              
        }

        private static void RunTutorial(string contentPath, Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,                                 
                                  TextControlManager textControlManager, IShellLocks shellLocks, IEditorManager editorManager, 
                                  DocumentManager documentManager, IUIApplication environment, IActionManager actionManager,
                                  ToolWindowManager toolWindowManager, TutorialWindowDescriptor tutorialWindowDescriptor,
                                  IWindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
                                  IColorThemeManager colorThemeManager, IThreading threading)
        {
            threading.ExecuteOrQueue("RunTutorialWindow", () => {
                var tutorialWindow = new TutorialWindow(contentPath, lifetime, solution, psiFiles, textControlManager, shellLocks, editorManager, documentManager, environment, actionManager, toolWindowManager, tutorialWindowDescriptor, windowsHookManager, psiServices, shortcutManager, colorThemeManager);

                lifetime.AddBracket(
                    () => { tutorialWindow.Show(); },
                    () => { tutorialWindow.Close(); });
            });            
        }   
    }
}
