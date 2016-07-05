using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application;
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

namespace ReSharperTutorials.TutWindow
{
    public class TutorialWindow : IStepView
    {
        private const string HtmlDoctype = "<!DOCTYPE html>";
//        private const string HtmlDoctype = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
        private const string HtmlHead = @"
        <HTML>
        <HEAD>
        <TITLE></TITLE>
        <style type='text/css'>
            html, 
            body 
            { 
                font-family:Tahoma, Geneva, sans-serif; font-size:100%; color:FNTCLR;
                overflow: auto;
                scrollbar-face-color: SCRLFACECLR;
                scrollbar-highlight-color: SCRLHLCLR;
                scrollbar-3dlight-color: SCRLHLCLR;
                scrollbar-darkshadow-color: SCRLHLCLR;
                scrollbar-shadow-color: SCRLHLCLR;
                scrollbar-arrow-color: SCRLARROWCLR;
                scrollbar-base-color: SCRLTRCKCLR;             
            }
            p { text-align:justify }          
            .done { text-decoration:line-through; color:DISFNT; } 
            .nextButton 
            {
                float: right;
//                background-color: BTNBCKCLR; 
//                border: none;
//                color: BTNFORECLR;
//                padding: 5px 10px;
//                text-align: center;
//                text-decoration: none;
//                display: inline-block;
//                font-size: 12px;   
//                border-radius: 2px;
//                cursor: pointer;                      
            } 
//            .nextButton:hover { background-color: BTNHVRCLR; }
            #prevStep { text-decoration:line-through; color:DISFNT; position: absolute; padding-right: 10px; padding-left: 10px;}            
            #currentStep { position: absolute; padding-right: 10px; padding-left: 10px;}
        </style>
        ";

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
        private readonly Lifetime _lifetime;
        private readonly IColorThemeManager _colorThemeManager;
        private string _textColor;
        private readonly ToolWindowClass _toolWindowClass;
        private string _scrollBackColor;
        private string _scrollFaceColor;
        private string _disabledTextColor;
        private string _buttonBackColor;
        private string _buttonForeColor;
        private string _buttonHoverColor;
        private LifetimeDefinition _animationLifetime;
        private HtmlMediator _htmlMediator;
        private CustomProgressBar _progressBar = new CustomProgressBar { Visible = true, Step = 1, Value = 0, Dock = DockStyle.Bottom,};

        public int StepCount
        {            
            set { _progressBar.Maximum = value; }
        }

        public event EventHandler NextStep;

        public string TutorialText { set { PrepareHtmlContent(value); } }

        public string StepText
        {
            get { return _stepText; }
            set
            {                
                // DIRTY HACK!
                if (_stepText != null && _stepText.Contains("prevStep"))
                {
                    _animationLifetime = Lifetimes.Define(_lifetime);

                    _htmlMediator.AllAnimationsDone.Advise(_animationLifetime.Lifetime, () =>
                    {
                        _stepText = value;
                        _shellLocks.ExecuteOrQueue(_lifetime, "TutorialTextUpdate",
                            () => { _viewControl.DocumentText = PrepareHtmlContent(_stepText); });

                        _animationLifetime.Terminate();
                    });
                    
                    _htmlMediator.Animate();
                }
                else
                {
                    _stepText = value;
                    _shellLocks.ExecuteOrQueue(_lifetime, "TutorialTextUpdate",
                        () => { _viewControl.DocumentText = PrepareHtmlContent(_stepText); });
                }                               
            }
        }        

        
        public TutorialWindow(string contentPath, Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
                                  TextControlManager textControlManager, IShellLocks shellLocks, IEditorManager editorManager,
                                  DocumentManager documentManager, IUIApplication environment, IActionManager actionManager,
                                  ToolWindowManager toolWindowManager, TutorialWindowDescriptor toolWindowDescriptor,
                                  IWindowsHookManager windowsHookManager, IPsiServices psiServices, IActionShortcuts shortcutManager,
                                  IColorThemeManager colorThemeManager)
        {
            _lifetime = lifetime;
            _solution = solution;
            _actionManager = actionManager;
            _shellLocks = shellLocks;
            _psiServices = psiServices;
            _shortcutManager = shortcutManager;
            _colorThemeManager = colorThemeManager;
            _toolWindowClass = toolWindowManager.Classes[toolWindowDescriptor];

            if (solution.GetComponent<ISolutionOwner>().IsRealSolutionOwner)
            {
                var toolWindowInstance = _toolWindowClass.RegisterInstance(
                    lifetime, null, null,
                    (lt, twi) =>
                    {
                        var containerControl = new TutorialPanel(environment).BindToLifetime(lt);                                                
                                                                                            
                        var viewControl = new HtmlViewControl(windowsHookManager, actionManager)
                        {                            
                            Dock = DockStyle.Fill, 
                            WebBrowserShortcutsEnabled = false,
                        }.BindToLifetime(lt);                                                                    

                        lt.AddBracket(
                            () => _containerControl = containerControl,
                            () => _containerControl = null);

                        lt.AddAction(() => _progressBar = null);

                        lt.AddBracket(
                            () => _containerControl.Controls.Add(_progressBar),
                            () => _containerControl.Controls.Remove(_progressBar));

                        lt.AddBracket(
                            () => _viewControl = viewControl,
                            () => _viewControl = null);                                                

                        lt.AddBracket(
                            () => _containerControl.Controls.Add(_viewControl),
                            () => _containerControl.Controls.Remove(_viewControl));

                        _colorThemeManager.ColorThemeChanged.Advise(lifetime, RefreshKeepContent);

                        SetColors();                        

                        _htmlMediator = new HtmlMediator(lifetime, _viewControl);
                        _htmlMediator.OnButtonClick.Advise(lifetime, () => NextStep?.Invoke(null, EventArgs.Empty));

                        return new EitherControl(lt, containerControl);
                    });
                
                _toolWindowClass.QueryCloseInstances.Advise(_lifetime, args => {Close();} );    // not working

                _stepPresenter = new TutorialStepPresenter(this, contentPath, lifetime, solution, psiFiles, textControlManager, 
                    shellLocks, editorManager, documentManager, environment, actionManager, psiServices, shortcutManager);
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
            foreach (var toolWindowInstance in _toolWindowClass.Instances)            
                toolWindowInstance.EnsureControlCreated().Show();                                    
        }


        public void Close()
        {
            foreach (var toolWindowInstance in _toolWindowClass.Instances)            
                toolWindowInstance.Close();
            
            _toolWindowClass.Close();
            VsIntegration.CloseVsSolution(true);
        }


        private void RefreshKeepContent()
        {
            StepText = _stepText;
        }


        private void SetColors()
        {            
            var backViewColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowBackground);
            backViewColor.ForEachValue(_lifetime, (lt, color) =>_viewControl.BackColor = color.GDIColor);

            var foreViewColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowForeground);
            foreViewColor.ForEachValue(_lifetime, (lt, color) => _viewControl.ForeColor = color.GDIColor);

            var backControlColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowBackground);
            backControlColor.ForEachValue(_lifetime, (lt, color) => _containerControl.BackColor = color.GDIColor);

            var foreControlColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ToolWindowForeground);
            foreControlColor.ForEachValue(_lifetime, (lt, color) => _containerControl.ForeColor = color.GDIColor);            

        }


        private static string GetResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();            
            var resourceNames = assembly.GetManifestResourceNames();

            var resourceName = (from name in resourceNames
                where name.Contains(fileName)
                select name).FirstOrDefault();
            
            string result;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)                
                    using (var reader = new StreamReader(stream))                    
                        result = reader.ReadToEnd();                                    
                else
                    throw new Exception("Unable to find content. Please reinstall the plugin");
            }
            return result;
        }


        private string PrepareHtmlContent(string content)
        {
            var html = new StringBuilder();
            BuildHeader(html);
            html.Append(content);
            BuildFooter(html);
            return html.ToString();
        }


        private static string ColorAsHtmlRgb(EitherColor color)
        {
            var r = color.GDIColor.R;
            var g = color.GDIColor.G;
            var b = color.GDIColor.B;
            return $"rgb({r},{g},{b})";
        }


        private void BuildHeader(StringBuilder html)
        {
            var fontColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.WindowText);
            fontColor.ForEachValue(_lifetime, (lt, color) => _textColor = ColorAsHtmlRgb(color));

            var disabledFontColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.DisabledText);
            disabledFontColor.ForEachValue(_lifetime, (lt, color) => _disabledTextColor = ColorAsHtmlRgb(color));

            var scrollBackColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ScrollBarBackground);
            scrollBackColor.ForEachValue(_lifetime, (lt, color) => _scrollBackColor = ColorAsHtmlRgb(color));

            var scrollFaceColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.TabStripButtonForeground);
            scrollFaceColor.ForEachValue(_lifetime, (lt, color) => _scrollFaceColor = ColorAsHtmlRgb(color));

            var buttonBackColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ContextMenuIconBackgroundGradientMiddle);
            buttonBackColor.ForEachValue(_lifetime, (lt, color) => _buttonBackColor = ColorAsHtmlRgb(color));

            var buttonForeColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.TabStripButtonForeground);
            buttonForeColor.ForEachValue(_lifetime, (lt, color) => _buttonForeColor = ColorAsHtmlRgb(color));

            var buttonHoverColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.ContextMenuItemMouseOverBackgroundGradientMiddle1);
            buttonHoverColor.ForEachValue(_lifetime, (lt, color) => _buttonHoverColor = ColorAsHtmlRgb(color));

            html.AppendLine(HtmlDoctype);
            html.AppendLine(HtmlHead);
            BuildScript(html);
            html.AppendLine("<BODY onload=\"stepLoad()\">");
            html.Replace("FNTCLR", _textColor);  // step text color
            html.Replace("DISFNT", _disabledTextColor);  // disabled step text color
            html.Replace("SCRLFACECLR", _scrollFaceColor);  // scrollbar tracker 
            html.Replace("SCRLARROWCLR", _scrollFaceColor); // scrollbar arrows
            html.Replace("SCRLHLCLR", _scrollBackColor);  // scrollbar background                        
            html.Replace("BTNBCKCLR", _buttonBackColor);  // button background
            html.Replace("BTNFORECLR", _buttonForeColor);  // button text                        
            html.Replace("BTNHVRCLR", _buttonForeColor);  // button text                        
        }


        private void BuildFooter(StringBuilder html)
        {
            html.AppendLine("</BODY>");
            html.AppendLine("</HTML>");
        }

        private void BuildScript(StringBuilder html)
        {
            html.AppendLine("<script>");
            html.AppendLine(GetResource("Scripts.js"));
            html.AppendLine("</script>");
            html.AppendLine("</ HEAD >");
        }

    }
}
