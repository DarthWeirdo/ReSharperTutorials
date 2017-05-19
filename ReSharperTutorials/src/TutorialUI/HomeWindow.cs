using System;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.CommonControls.Browser;
using JetBrains.DataFlow;
using JetBrains.Threading;
using JetBrains.UI.Application;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Extensions;
using JetBrains.UI.ToolWindowManagement;

namespace ReSharperTutorials.TutorialUI
{
    class HomeWindow : IHtmlCommunication
    {
        private readonly TutorialWindowManager _windowManager;
        private readonly TabbedToolWindowClass _toolWindowClass;
        private readonly IShellLocks _shellLocks;
        private TutorialPanel _containerControl;
        private HtmlViewControl _viewControl = new HtmlViewControl(null, null);
        private readonly Lifetime _lifetime;
        private readonly IColorThemeManager _colorThemeManager;
        private string _pageText;
        private readonly HtmlGenerator _htmlGenerator;
        private HtmlMediator _htmlMediator;
        public HtmlMediator HtmlMediator => _htmlMediator;
        public HtmlViewControl HtmlViewControl => _viewControl;
        public Lifetime WindowLifetime => _lifetime;

        public string PageText
        {
            set
            {
                _pageText = value;
                _shellLocks.ExecuteOrQueue(_lifetime, "HomeTextUpdate",
                    () => { _viewControl.DocumentText = _htmlGenerator.PrepareHtmlContent(_pageText); });
            }

            get { return _pageText; }
        }


        public HomeWindow(Lifetime lifetime, TutorialWindowManager windowManager,
            IShellLocks shellLocks, IUIApplication environment, IActionManager actionManager,
            TabbedToolWindowClass toolWindowClass, IWindowsHookManager windowsHookManager,
            IColorThemeManager colorThemeManager)
        {
            _windowManager = windowManager;
            _lifetime = lifetime;
            _shellLocks = shellLocks;
            _colorThemeManager = colorThemeManager;
            _toolWindowClass = toolWindowClass;
            _htmlGenerator = new HtmlGenerator(lifetime, colorThemeManager);

            var toolWindowInstance = _toolWindowClass.RegisterInstance(
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

                    _htmlMediator = new HtmlMediator(lifetime, this);

                    _colorThemeManager.ColorThemeChanged.Advise(lifetime, RefreshKeepContent);

                    SetColors();

                    return new EitherControl(lt, containerControl);
                });

            toolWindowInstance.Title.Value = "Home";
        }

        public void EnableButtons(bool state)
        {
            _htmlMediator?.EnableButtons(state);
        }

        public void HideLoadingImages()
        {
            _htmlMediator?.HideImages();
        }

        public void AgreeToRunTutorial(int tutorialId, string imgSrc)
        {
            _htmlMediator?.AgreeToRunTutorial(tutorialId.ToString(), imgSrc);
        }


        public void Show()
        {
            foreach (var toolWindowInstance in _toolWindowClass.Instances)
                toolWindowInstance.EnsureControlCreated().Show();
        }


        private void SetColors()
        {
            var backViewColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowBackground);
            backViewColor.ForEachValue(_lifetime, (lt, color) => _viewControl.BackColor = color.GDIColor);

            var foreViewColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowForeground);
            foreViewColor.ForEachValue(_lifetime, (lt, color) => _viewControl.ForeColor = color.GDIColor);

            var backControlColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowBackground);
            backControlColor.ForEachValue(_lifetime, (lt, color) => _containerControl.BackColor = color.GDIColor);

            var foreControlColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowForeground);
            foreControlColor.ForEachValue(_lifetime, (lt, color) => _containerControl.ForeColor = color.GDIColor);
        }


        private void RefreshKeepContent(bool obj)
        {
            PageText = _pageText;
        }

        public void RunTutorial(string htmlTutorialId)
        {
            _windowManager.RunTutorial(Convert.ToInt32(htmlTutorialId));
        }
    }
}