﻿using System;
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
using Button = System.Windows.Forms.Button;
using Color = System.Drawing.Color;

namespace ReSharperTutorials.TutWindow
{
    public class TutorialWindow : IStepView
    {
        private const string HtmlDoctype = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
        private const string HtmlHead = @"
        <HTML>
        <HEAD>
        <TITLE></TITLE>
        <style type='text/css'>
            html, 
            body 
            { 
                font-family:tahoma, sans-serif; font-size:90%; color:FNTCLR;
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
            #prevStep { text-decoration:line-through; color:DISFNT; position: absolute;}            
            #currentStep { position: absolute;}
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
        private Button _buttonNext = new Button();
        private TutorialStepPresenter _stepPresenter;
        private readonly Lifetime _lifetime;
        private readonly IColorThemeManager _colorThemeManager;
        private string _textColor;
        private readonly ToolWindowClass _toolWindowClass;
        private string _scrollBackColor;
        private string _scrollFaceColor;
        private string _disabledTextColor;


        public string TutorialText { set { PrepareHtmlContent(value); } }

        public string StepText
        {
            get { return _stepText; }
            set
            {                
                // DIRTY HACK!
                if (_stepText != null && _stepText.Contains("prevStep"))
                {                    
                    var animLifetime = Lifetimes.Define(_lifetime);
                    var mediator = new HtmlMediator(animLifetime.Lifetime, _viewControl);                    
                    mediator.AllAnimationsDone.Advise(animLifetime.Lifetime, () =>
                    {
                        _stepText = value;
                        _shellLocks.ExecuteOrQueue(_lifetime, "TutorialTextUpdate",
                            () => { _viewControl.DocumentText = PrepareHtmlContent(_stepText); });

                        animLifetime.Terminate();
                    });

                    mediator.Animate();
                }
                else
                {
                    _stepText = value;
                    _shellLocks.ExecuteOrQueue(_lifetime, "TutorialTextUpdate",
                        () => { _viewControl.DocumentText = PrepareHtmlContent(_stepText); });
                }                               
            }
        }

        public string ButtonText
        {
            get { return _buttonNext.Text; }
            set { _buttonNext.Text = value; }
        }

        public bool ButtonVisible
        {
            get { return _buttonNext.Visible; }
            set { _buttonNext.Visible = value; }
        }

        public event EventHandler NextStep;


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
                        }.BindToLifetime(lt);                                                   

                        var buttonNext = new Button
                        {
                            Text = "Next",
                            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                            FlatStyle = FlatStyle.Flat,
                            FlatAppearance = {BorderColor = Color.Gray, BorderSize = 1}                                                        
                        };                        

                        lt.AddBracket(
                            () => _buttonNext = buttonNext,
                            () => _buttonNext = null);

                        _buttonNext.Top = containerControl.Height - _buttonNext.Height - 10;
                        _buttonNext.Left = containerControl.Width - _buttonNext.Width - 25;

                        lt.AddBracket(
                            () => _containerControl = containerControl,
                            () => _containerControl = null);

                        lt.AddBracket(
                            () => _viewControl = viewControl,
                            () => _viewControl = null);                                                

                        lt.AddBracket(
                            () => { _buttonNext.Click += NextStep; },
                            () => { _buttonNext.Click -= NextStep; });

                        lt.AddBracket(
                            () => _containerControl.Controls.Add(_buttonNext),
                            () => _containerControl.Controls.Remove(_buttonNext));

                        lt.AddBracket(
                            () => _containerControl.Controls.Add(_viewControl),
                            () => _containerControl.Controls.Remove(_viewControl));

                        _colorThemeManager.ColorThemeChanged.Advise(lifetime, RefreshKeepContent);

                        SetColors();                        

                        return new EitherControl(lt, containerControl);
                    });
                
                _toolWindowClass.QueryCloseInstances.Advise(_lifetime, args => {Close();} );    // not working

                _stepPresenter = new TutorialStepPresenter(this, contentPath, lifetime, solution, psiFiles, textControlManager, 
                    shellLocks, editorManager, documentManager, environment, actionManager, psiServices, shortcutManager);
            }
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
            VsCommunication.CloseVsSolution(true);
        }


        public void RefreshKeepContent()
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

            var buttonBackColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.TabStripButtonBackground);
            buttonBackColor.ForEachValue(_lifetime, (lt, color) => _buttonNext.BackColor = color.GDIColor);

            var buttonForeColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.TabStripButtonForeground);
            buttonForeColor.ForEachValue(_lifetime, (lt, color) => _buttonNext.ForeColor = color.GDIColor);            

        }


        private string GetJsScript(string fileName)
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

            html.AppendLine(HtmlDoctype);
            html.AppendLine(HtmlHead);
            BuildScript(html);
            html.AppendLine("<BODY onload=\"stepLoad()\">");
            html.Replace("FNTCLR", _textColor);  // step text color
            html.Replace("DISFNT", _disabledTextColor);  // disabled step text color
            html.Replace("SCRLFACECLR", _scrollFaceColor);  // scrollbar tracker 
            html.Replace("SCRLARROWCLR", _scrollFaceColor); // scrollbar arrows
            html.Replace("SCRLHLCLR", _scrollBackColor);  // scrollbar background                        
        }


        private void BuildFooter(StringBuilder html)
        {
            html.AppendLine("</BODY>");
            html.AppendLine("</HTML>");
        }

        private void BuildScript(StringBuilder html)
        {
            html.AppendLine("<script>");
            html.AppendLine(GetJsScript("Scripts.js"));
            html.AppendLine("</script>");
            html.AppendLine("</ HEAD >");
        }

    }
}