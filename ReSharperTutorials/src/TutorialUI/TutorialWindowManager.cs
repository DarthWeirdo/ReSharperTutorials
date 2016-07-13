using System;
using System.Linq;
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
using ReSharperTutorials.Runner;
using ReSharperTutorials.Utils;
using MessageBox = JetBrains.Util.MessageBox;

namespace ReSharperTutorials.TutorialUI
{
    public class TutorialWindowManager
    {
        private readonly TabbedToolWindowClass _toolWindowClass;
        private readonly Lifetime _shellLifetime;
        private readonly IShellLocks _shellLocks;
        private TutorialId _runningTutorial;
        private readonly IUIApplication _environment;
        private readonly IActionManager _actionManager;
        private readonly IWindowsHookManager _windowsHookManager;
        private readonly IColorThemeManager _colorThemeManager;
        private readonly IThreading _threading;
        private HomeWindow _homeWindow = null;
        private TutorialWindow _tutorialWindow = null;

        public TutorialWindowManager(Lifetime shellLifetime, IShellLocks shellLocks, ToolWindowManager toolWindowManager, 
            TutorialWindowDescriptor toolWindowDescriptor, IUIApplication environment, IActionManager actionManager, 
            IWindowsHookManager windowsHookManager, IColorThemeManager colorThemeManager, IThreading threading)
        {
            _shellLifetime = shellLifetime;
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


        public void ShowTutorialWindow(GlobalSettings globalSettings, TutorialId tutorialId, Lifetime lifetime, 
            ISolution solution, IPsiFiles psiFiles, TextControlManager textControlManager, IShellLocks shellLocks, 
            IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment, 
            IActionManager actionManager, IWindowsHookManager windowsHookManager, IPsiServices psiServices, 
            IActionShortcuts shortcutManager, IColorThemeManager colorThemeManager, IThreading threading)
        {
            var contentPath = globalSettings.GetPath(tutorialId, PathType.WorkCopyContentFile);
            _runningTutorial = tutorialId;

            threading.ExecuteOrQueue("RunTutorialWindow", () =>
            {
                _tutorialWindow = new TutorialWindow(contentPath, lifetime, solution, psiFiles, textControlManager,
                    shellLocks, editorManager, documentManager, environment, actionManager, _toolWindowClass, windowsHookManager, psiServices, shortcutManager, colorThemeManager);

                lifetime.AddBracket(
                    () => _tutorialWindow.Show(),
                    () =>
                    {
                        _tutorialWindow.Close();                        
                        _tutorialWindow = null; // TODO: do we need it? 
                        _runningTutorial = TutorialId.None;
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
                _homeWindow = new HomeWindow(_shellLifetime, _shellLocks, _environment, _actionManager, _toolWindowClass, _windowsHookManager, _colorThemeManager);

                _homeWindow.Show();
            });
        }        
    }
}
