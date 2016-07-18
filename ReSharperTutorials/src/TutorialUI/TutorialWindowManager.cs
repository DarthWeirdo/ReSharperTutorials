using System;
using JetBrains.ActionManagement;
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
        private TutorialId _runningTutorial;
        private TutorialWindow _tutorialWindow;
        private readonly GlobalSettings _globalSettings;

        public TutorialWindowManager(Lifetime shellLifetime, GlobalSettings globalSettings, IShellLocks shellLocks, 
            ToolWindowManager toolWindowManager, TutorialWindowDescriptor toolWindowDescriptor, IUIApplication environment, 
            IActionManager actionManager, IWindowsHookManager windowsHookManager, IColorThemeManager colorThemeManager, 
            IThreading threading)
        {
            _shellLifetime = shellLifetime;
            _globalSettings = globalSettings;
            _shellLocks = shellLocks;
            _environment = environment;
            _actionManager = actionManager;
            _windowsHookManager = windowsHookManager;
            _colorThemeManager = colorThemeManager;
            _threading = threading;

            _runningTutorial = TutorialId.None;

            _toolWindowClass = toolWindowManager.Classes[toolWindowDescriptor] as TabbedToolWindowClass;
            if (_toolWindowClass == null)
                throw new ApplicationException("Expected tabbed tool window");

            _toolWindowClass.QueryCloseInstances.Advise(shellLifetime, args =>
            {
                if (_runningTutorial == TutorialId.None) return;
                args.Cancel = !MessageBox.ShowYesNo(
                    "This will close the tutorial solution as well. Tutorial progress will be lost. Close the tutorial?",
                    "ReSharper Tutorials");
                if (args.Cancel) return;
                VsIntegration.CloseVsSolution(true);
            });
        }


        public bool WindowsExist()
        {
            return _toolWindowClass.Instances.Length > 0;
        }        


        public void ShowTutorialWindow(TutorialId tutorialId, Lifetime lifetime,
            ISolution solution, IPsiFiles psiFiles, TextControlManager textControlManager, IShellLocks shellLocks,
            IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment,
            IActionManager actionManager, IWindowsHookManager windowsHookManager, IPsiServices psiServices,
            IActionShortcuts shortcutManager, IColorThemeManager colorThemeManager, IThreading threading)
        {
            var contentPath = _globalSettings.GetPath(tutorialId, PathType.WorkCopyContentFile);            
            _runningTutorial = tutorialId;                       

            threading.ExecuteOrQueue("RunTutorialWindow", () =>
            {                
                _tutorialWindow = new TutorialWindow(contentPath, lifetime, solution, psiFiles, textControlManager,
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
                        _runningTutorial = TutorialId.None;
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
                _homeWindow = new HomeWindow(_shellLifetime, _globalSettings, _shellLocks, _environment, _actionManager, _toolWindowClass,
                    _windowsHookManager, _colorThemeManager)
                {
                    PageText = HtmlGenerator.GetResource("HomePage.html")
                };

                _homeWindow.Show();
            });
        }
    }
}