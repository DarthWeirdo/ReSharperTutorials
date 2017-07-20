using System;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.changes;
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
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Runner
{
    [SolutionComponent]
    public class TutorialWindowRunner
    {
        public TutorialWindowRunner([NotNull] Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
            ChangeManager changeManager, [NotNull] ISolutionStateTracker solutionStateTracker,
            [NotNull] GlobalSettings globalSettings, TextControlManager textControlManager, IShellLocks shellLocks,
            IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment,
            IActionManager actionManager,
            WindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
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
                if (VsIntegration.GetCurrentSolutionPath() == tutorial.Value)
                {
                    solutionStateTracker.AfterSolutionOpened.Advise(lifetime,
                        () =>
                            RunTutorial(globalSettings, tutorial.Key, lifetime, solution, psiFiles, changeManager,
                                textControlManager, shellLocks, editorManager, documentManager, environment,
                                actionManager, windowsHookManager, psiServices, shortcutManager, colorThemeManager,
                                threading));
                }
            }
        }

        private static void RunTutorial(GlobalSettings globalSettings, int tutorialId, Lifetime lifetime,
            ISolution solution,
            IPsiFiles psiFiles, ChangeManager changeManager, TextControlManager textControlManager,
            IShellLocks shellLocks,
            IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment,
            IActionManager actionManager,
            WindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
            IColorThemeManager colorThemeManager, IThreading threading)
        {
            if (globalSettings.TutorialWindowManager == null)
                throw new ApplicationException("Expected globalSettings.TutorialWindowManager");

            globalSettings.TutorialWindowManager.ShowTutorialWindow(tutorialId, lifetime, solution, psiFiles,
                changeManager,
                textControlManager, shellLocks, editorManager, documentManager, environment, actionManager,
                windowsHookManager, psiServices, shortcutManager, colorThemeManager, threading);
        }
    }
}