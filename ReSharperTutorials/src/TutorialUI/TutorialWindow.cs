using System;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.Application.DataContext;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.Application.Settings;
using JetBrains.CommonControls.Browser;
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
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Extensions;
using JetBrains.UI.ToolWindowManagement;
using ReSharperTutorials.TutStep;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.TutorialUI
{
    public class TutorialWindow : IStepView, IHtmlCommunication
    {
        private readonly TutorialWindowManager _windowManager;
        private readonly IPsiServices _psiServices;
        private readonly IActionShortcuts _shortcutManager;
        private readonly ISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;
        private readonly ISolution _solution;
        private readonly IActionManager _actionManager;
        private readonly IShellLocks _shellLocks;
        private TutorialPanel _containerControl;
        private HtmlViewControl _viewControl = new HtmlViewControl(null, null);
        private string _stepText;
        private TutorialStepPresenter _stepPresenter;
        private readonly Lifetime _tutorialLifetime;
        private readonly IColorThemeManager _colorThemeManager;
        private readonly TabbedToolWindowClass _toolWindowClass;
        private readonly ToolWindowInstance _toolWindowInstance;
        private LifetimeDefinition _animationLifetime;
        private HtmlMediator _htmlMediator;
        private WindowFocusTracker _focusTracker;

        private CustomProgressBar _progressBar = new CustomProgressBar
        {
            Visible = true,
            Step = 1,
            Value = 0,
            Dock = DockStyle.Bottom
        };

        private readonly HtmlGenerator _htmlGenerator;
        public HtmlMediator HtmlMediator => _htmlMediator;
        public HtmlViewControl HtmlViewControl => _viewControl;
        public bool IsLastStep => _stepPresenter.IsLastStep;

        public int StepCount
        {
            set { _progressBar.Maximum = value; }
        }

        public event EventHandler NextStep;

        public string StepText
        {
            get { return _stepText; }
            set
            {
                if (_stepText != null && _stepText.Contains("prevStep"))
                {
                    _animationLifetime = Lifetimes.Define(_tutorialLifetime);

                    _htmlMediator?.AllAnimationsDone.Advise(_animationLifetime.Lifetime, () =>
                    {
                        _stepText = value;
                        _shellLocks.ExecuteOrQueue(_tutorialLifetime, "TutorialTextUpdate",
                            () => { _viewControl.DocumentText = _htmlGenerator.PrepareHtmlContent(_stepText); });
                        //_shellLocks.Dispatcher.Invoke("sdf",() => {},);
                        _animationLifetime.Terminate();
                    });

                    _htmlMediator?.Animate();
                }
                else
                {
                    _stepText = value;
                    _shellLocks.ExecuteOrQueue(_tutorialLifetime, "TutorialTextUpdate",
                        () => { _viewControl.DocumentText = _htmlGenerator.PrepareHtmlContent(_stepText); });
                }
            }
        }


        public TutorialWindow(string contentPath, Lifetime tutorialLifetime, TutorialWindowManager windowManager,
            ISolution solution, IPsiFiles psiFiles,
            ChangeManager changeManager, TextControlManager textControlManager, IShellLocks shellLocks,
            IEditorManager editorManager,
            DocumentManager documentManager, IUIApplication environment, IActionManager actionManager,
            TabbedToolWindowClass toolWindowClass,
            WindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
            IColorThemeManager colorThemeManager)
        {
            _windowManager = windowManager;
            _htmlGenerator = new HtmlGenerator(tutorialLifetime, colorThemeManager);
            _tutorialLifetime = tutorialLifetime;
            _solution = solution;
            _actionManager = actionManager;
            _shellLocks = shellLocks;
            _psiServices = psiServices;
            _shortcutManager = shortcutManager;
            _colorThemeManager = colorThemeManager;
            _toolWindowClass = toolWindowClass;

            if (solution.GetComponent<ISolutionOwner>().IsRealSolutionOwner)
            {
                _toolWindowInstance = _toolWindowClass.RegisterInstance(
                    tutorialLifetime, null, null,
                    (lt, twi) =>
                    {
                        twi.QueryClose.Value = true;

                        var containerControl = new TutorialPanel(environment).BindToLifetime(lt);

                        var viewControl = new HtmlViewControl(windowsHookManager, actionManager)
                        {
                            Dock = DockStyle.Fill,
                            WebBrowserShortcutsEnabled = false
                        }.BindToLifetime(lt);

                        lt.AddBracket(
                            () => _containerControl = containerControl,
                            () => _containerControl = null);

                        lt.AddAction(() => _progressBar = null);

                        lt.AddBracket(
                            () => _viewControl = viewControl,
                            () => _viewControl = null);

                        lt.AddBracket(
                            () =>
                            {
                                _containerControl.Controls.Add(_viewControl);
                                _containerControl.Controls.Add(_progressBar);
                            },
                            () =>
                            {
                                _containerControl.Controls.Remove(_viewControl);
                                _containerControl.Controls.Remove(_progressBar);
                            });

                        _colorThemeManager.ColorThemeChanged.Advise(tutorialLifetime, RefreshKeepContent);

                        SetColors();

                        _htmlMediator = new HtmlMediator(tutorialLifetime, this);
                        _htmlMediator.OnNextStepButtonClick.Advise(tutorialLifetime,
                            () => NextStep?.Invoke(null, EventArgs.Empty));
                        _htmlMediator.OnRunStepNavigationLinkClick.Advise(tutorialLifetime, NavigateToCodeByLink);

                        _focusTracker = new WindowFocusTracker(tutorialLifetime);
                        _focusTracker.IsFocusOnEditor.Change.Advise(tutorialLifetime,
                            () => _htmlMediator.ChangeNextStepButtonText(_focusTracker.IsFocusOnEditor.Value));

                        _htmlMediator.OnPageHasFullyLoaded.Advise(tutorialLifetime,
                            () => { _htmlMediator.ChangeNextStepButtonText(_focusTracker.IsFocusOnEditor.Value); });

                        return new EitherControl(lt, containerControl);
                    });

                _stepPresenter = new TutorialStepPresenter(this, contentPath, tutorialLifetime, solution, psiFiles,
                    changeManager,
                    textControlManager, shellLocks, editorManager, documentManager, environment, actionManager,
                    psiServices, shortcutManager);

                _toolWindowInstance.Title.Value = _stepPresenter.Title;
            }
        }


        public void UpdateProgress()
        {
            _progressBar.PerformStep();
            _progressBar.CustomText = $"Step {_progressBar.Value} of {_progressBar.Maximum}";
            _progressBar.Refresh();
        }


        public void Show()
        {
            _toolWindowInstance.Show(true);
        }


        public void Close()
        {
            _toolWindowInstance.QueryClose.Value = false;
            _toolWindowInstance.Close();
        }

        private void NavigateToCodeByLink()
        {
            _stepPresenter.RunStepNavigation();
        }


        private void RefreshKeepContent()
        {
            StepText = _stepText;
        }


        private void SetColors()
        {
            var backViewColor = _colorThemeManager.CreateLiveColor(_tutorialLifetime, ThemeColor.ToolWindowBackground);
            backViewColor.ForEachValue(_tutorialLifetime, (lt, color) => _viewControl.BackColor = color.GDIColor);

            var foreViewColor = _colorThemeManager.CreateLiveColor(_tutorialLifetime, ThemeColor.ToolWindowForeground);
            foreViewColor.ForEachValue(_tutorialLifetime, (lt, color) => _viewControl.ForeColor = color.GDIColor);

            var backControlColor =
                _colorThemeManager.CreateLiveColor(_tutorialLifetime, ThemeColor.ToolWindowBackground);
            backControlColor.ForEachValue(_tutorialLifetime,
                (lt, color) => _containerControl.BackColor = color.GDIColor);

            var foreControlColor =
                _colorThemeManager.CreateLiveColor(_tutorialLifetime, ThemeColor.ToolWindowForeground);
            foreControlColor.ForEachValue(_tutorialLifetime,
                (lt, color) => _containerControl.ForeColor = color.GDIColor);
        }

        public void RunTutorial(string htmlTutorialId)
        {
            _windowManager.RunTutorial(Convert.ToInt32(htmlTutorialId));
        }
    }
}