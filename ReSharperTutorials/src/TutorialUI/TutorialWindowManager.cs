using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
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
using JetBrains.UI.ToolWindowManagement;
using JetBrains.Util;
using ReSharperTutorials.Runner;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.TutorialUI
{
    public class TutorialWindowManager
    {
        private readonly IActionManager _actionManager;
        private readonly IColorThemeManager _colorThemeManager;
        private readonly IUIApplication _environment;
        private readonly Lifetime _shellLifetime;
        private readonly IShellLocks _shellLocks;
        private readonly IThreading _threading;
        private readonly TabbedToolWindowClass _toolWindowClass;
        private readonly IWindowsHookManager _windowsHookManager;
        private HomeWindow _homeWindow;
        private int _runningTutorial;
        private TutorialWindow _tutorialWindow;
        private readonly GlobalSettings _globalSettings;
        private SolutionStateTracker _solutionStateTracker;

        public TutorialWindowManager(Lifetime shellLifetime, SolutionStateTracker solutionStateTracker,
            GlobalSettings globalSettings,
            IShellLocks shellLocks, ToolWindowManager toolWindowManager, TutorialWindowDescriptor toolWindowDescriptor,
            IUIApplication environment, IActionManager actionManager, IWindowsHookManager windowsHookManager,
            IColorThemeManager colorThemeManager, IThreading threading)
        {
            _shellLifetime = shellLifetime;
            _solutionStateTracker = solutionStateTracker;
            _globalSettings = globalSettings;
            _shellLocks = shellLocks;
            _environment = environment;
            _actionManager = actionManager;
            _windowsHookManager = windowsHookManager;
            _colorThemeManager = colorThemeManager;
            _threading = threading;

            _runningTutorial = 0;

            _toolWindowClass = toolWindowManager.Classes[toolWindowDescriptor] as TabbedToolWindowClass;
            if (_toolWindowClass == null)
                throw new ApplicationException("Expected tabbed tool window");

            _toolWindowClass.QueryCloseInstances.Advise(shellLifetime, args =>
            {
                if (_runningTutorial == 0) return;
                if (!_tutorialWindow.IsLastStep)
                {
                    args.Cancel = !MessageBox.ShowYesNo(
                        "This will close the tutorial solution as well. Tutorial progress will be lost. Close the tutorial?",
                        "ReSharper Tutorials");
                    if (args.Cancel) return;
                }
                VsIntegration.CloseVsSolution(true);
            });
        }


        public bool WindowsExist()
        {
            return _toolWindowClass.Instances.Length > 0;
        }


        public void ShowTutorialWindow(int tutorialId, Lifetime lifetime,
            ISolution solution, IPsiFiles psiFiles, ChangeManager changeManager, TextControlManager textControlManager,
            IShellLocks shellLocks, IEditorManager editorManager, DocumentManager documentManager,
            IUIApplication environment,
            IActionManager actionManager, WindowsHookManager windowsHookManager, IPsiServices psiServices,
            IActionShortcuts shortcutManager, IColorThemeManager colorThemeManager, IThreading threading)
        {
            var contentPath = _globalSettings.GetPath(tutorialId, PathType.WorkCopyContentFile);
            _runningTutorial = tutorialId;

            threading.ExecuteOrQueue("RunTutorialWindow", () =>
            {
                _tutorialWindow = new TutorialWindow(contentPath, lifetime, this, solution, psiFiles, changeManager,
                    textControlManager,
                    shellLocks, editorManager, documentManager, environment, actionManager, _toolWindowClass,
                    windowsHookManager, psiServices, shortcutManager, colorThemeManager);

                lifetime.AddBracket(
                    () =>
                    {
                        _tutorialWindow.Show();
                        _homeWindow.HideLoadingImages();
                    },
                    () =>
                    {
                        _tutorialWindow.Close();
                        _tutorialWindow = null;
                        _runningTutorial = 0;
                        _homeWindow.EnableButtons(true);
                    });
            });
        }


        public void ShowHomeWindow()
        {
            if (_homeWindow != null)
            {
                _homeWindow.Show();
                return;
            }

            _threading.ExecuteOrQueue("RunMainWindow", () =>
            {
                _homeWindow = new HomeWindow(_shellLifetime, this, _solutionStateTracker, _globalSettings, _shellLocks,
                    _environment,
                    _actionManager, _toolWindowClass, _windowsHookManager, _colorThemeManager)
                {
                    PageText = HtmlGenerator.GetResource("HomePage.html")
                };

                _homeWindow.Show();
            });
        }

        public void RunTutorial(int tutorialId)
        {
            try
            {
                EnvironmentChecker.RunAllChecks(tutorialId);
            }
            catch (NoShortcutsAssignedException e)
            {                
                var shList = (List<string>) e.Data["Shortcuts"];
                var listText = shList.Aggregate("\n", (current, sh) => current + sh + "\n");

                MessageBox.ShowError(
                    "Some of the ReSharper shortcuts are not assigned:" + listText +
                    "\nPlease assign these shortcuts in 'Tools | Options... | Environment | Keyboard' " + 
                    "or apply a keyboard scheme in 'ReSharper | Options... | Environment | Keyboard & Menus' " +
                    "before running the tutorial.",
                    "ReSharper Tutorials");
                return;
            }
            catch (NoShortcutSchemeSelectedException e)
            {
                MessageBox.ShowError(
                    "ReSharper shortcut scheme is not selected! Please select a scheme in " +
                    "ReSharper | Options... | Environment | Keyboard & Menus before running the tutorial.",
                    "ReSharper Tutorials");
                return;
            }

            var result =
                MessageBox.ShowYesNo(
                    "This will close your current solution and open the tutorial solution. Run the tutorial?",
                    "ReSharper Tutorials");
            if (!result) return;

            var loadingLifetime = Lifetimes.Define();
            _solutionStateTracker.AgreeToRunTutorial.Advise(loadingLifetime.Lifetime, () =>
            {
                var loadingImgPath = _globalSettings.GetGlobalImgPath() + "\\loading20x20.gif";
                _homeWindow.EnableButtons(false);
                _homeWindow.AgreeToRunTutorial(tutorialId, loadingImgPath);
            });

            _solutionStateTracker.AfterPsiLoaded.Advise(loadingLifetime.Lifetime, () => loadingLifetime.Terminate());

            // TODO: store id and action in dictionary, search dictionary for this id and run corresponding action
            switch (tutorialId)
            {
                case 1:
                    _shellLocks.ExecuteOrQueue(_homeWindow.WindowLifetime, "RunTutorial",
                        () => _actionManager.ExecuteAction<ActionOpenTutorial1>());
                    break;
                case 3:
                    _shellLocks.ExecuteOrQueue(_homeWindow.WindowLifetime, "RunTutorial",
                        () => _actionManager.ExecuteAction<ActionOpenTutorial3>());
                    break;
                case 4:
                    _shellLocks.ExecuteOrQueue(_homeWindow.WindowLifetime, "RunTutorial",
                        () => _actionManager.ExecuteAction<ActionOpenTutorial4>());
                    break;
            }
        }
    }
}