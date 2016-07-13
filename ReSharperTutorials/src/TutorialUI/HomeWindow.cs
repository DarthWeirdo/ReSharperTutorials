using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.CommonControls.Browser;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Extensions;
using JetBrains.UI.ToolWindowManagement;
using ReSharperTutorials.Runner;

namespace ReSharperTutorials.TutorialUI
{
    class HomeWindow
    {
        private readonly TabbedToolWindowClass _toolWindowClass;
        private readonly ToolWindowInstance _toolWindowInstance;
        private readonly IActionManager _actionManager;
        private readonly IShellLocks _shellLocks;
        private TutorialPanel _containerControl;
        private HtmlViewControl _viewControl = new HtmlViewControl(null, null);
        private readonly Lifetime _lifetime;
        private readonly IColorThemeManager _colorThemeManager;

        private TutorialId _runningTutorial;


        public HomeWindow(Lifetime lifetime, IShellLocks shellLocks, IUIApplication environment, IActionManager actionManager,
            TabbedToolWindowClass toolWindowClass, IWindowsHookManager windowsHookManager, IColorThemeManager colorThemeManager)
        {
            _lifetime = lifetime;            
            _actionManager = actionManager;
            _shellLocks = shellLocks;                      
            _colorThemeManager = colorThemeManager;
            _toolWindowClass = toolWindowClass;

            _toolWindowInstance = _toolWindowClass.RegisterInstance(
                lifetime, null, null,
                (lt, twi) =>
                {
                    twi.QueryClose.Value = true;

                    var containerControl = new TutorialPanel(environment).BindToLifetime(lt);

                    var viewControl = new HtmlViewControl(windowsHookManager, actionManager)
                    {
                        Dock = DockStyle.Fill,
                        WebBrowserShortcutsEnabled = false,
                    }.BindToLifetime(lt);

                    lt.AddBracket(
                        () => _containerControl = containerControl,
                        () => _containerControl = null);

                    lt.AddBracket(
                        () => _viewControl = viewControl,
                        () => _viewControl = null);

                    lt.AddBracket(
                        () => _containerControl.Controls.Add(_viewControl),
                        () => _containerControl.Controls.Remove(_viewControl));

                    _colorThemeManager.ColorThemeChanged.Advise(lifetime, RefreshKeepContent);

                    SetColors();

                    return new EitherControl(lt, containerControl);
                });
                        
            _toolWindowInstance.Title.Value = "Home";
            
        }

        public void Show()
        {
            foreach (var toolWindowInstance in _toolWindowClass.Instances)
                toolWindowInstance.EnsureControlCreated().Show();
        }

        private void SetColors()
        {
            
        }

        private void RefreshKeepContent(bool obj)
        {
            
        }
    }
}