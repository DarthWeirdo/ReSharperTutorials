using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.DataFlow;
using JetBrains.UI.Components.Theming;

namespace ReSharperTutorials.TutorialUI
{
    public class HtmlGenerator
    {
        private readonly Lifetime _lifetime;
        private readonly IColorThemeManager _colorThemeManager;
        private string _textColor;
        private string _scrollBackColor;
        private string _scrollFaceColor;
        private string _disabledTextColor;
        private string _buttonBackColor;
        private string _buttonForeColor;
        private string _buttonHoverColor;
        private string _codeColor;
        private const string HtmlDoctype = "<!DOCTYPE html>";
        private const string HtmlHead = @"<HTML><HEAD><TITLE></TITLE>";

        public HtmlGenerator(Lifetime lifetime, IColorThemeManager colorThemeManager)
        {
            _lifetime = lifetime;
            _colorThemeManager = colorThemeManager;
        }

        public static string GetResource(string fileName)
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

        public string PrepareHtmlContent(string content)
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

            var buttonBackColor = _colorThemeManager.CreateLiveColor(_lifetime,
                ThemeColor.ContextMenuIconBackgroundGradientMiddle);
            buttonBackColor.ForEachValue(_lifetime, (lt, color) => _buttonBackColor = ColorAsHtmlRgb(color));

            var buttonForeColor = _colorThemeManager.CreateLiveColor(_lifetime, ThemeColor.TabStripButtonForeground);
            buttonForeColor.ForEachValue(_lifetime, (lt, color) => _buttonForeColor = ColorAsHtmlRgb(color));

            var buttonHoverColor = _colorThemeManager.CreateLiveColor(_lifetime,
                ThemeColor.ContextMenuItemMouseOverBackgroundGradientMiddle1);
            buttonHoverColor.ForEachValue(_lifetime, (lt, color) => _buttonHoverColor = ColorAsHtmlRgb(color));

            var codeColor = _colorThemeManager.CreateLiveColor(_lifetime,
                ThemeColor.ToolWindowSelectedInactiveTreeItemBackground);
            codeColor.ForEachValue(_lifetime, (lt, color) => _codeColor = ColorAsHtmlRgb(color));

            html.AppendLine(HtmlDoctype);
            html.AppendLine(HtmlHead);
            BuildCss(html);
            BuildScript(html);
            html.AppendLine("<BODY onload=\"stepLoad()\">");
            html.Replace("FNTCLR", _textColor); // step text color
            html.Replace("DISFNT", _disabledTextColor); // disabled step text color
            html.Replace("SCRLFACECLR", _scrollFaceColor); // scrollbar tracker 
            html.Replace("SCRLARROWCLR", _scrollFaceColor); // scrollbar arrows
            html.Replace("SCRLHLCLR", _scrollBackColor); // scrollbar background                        
            html.Replace("BTNBCKCLR", _buttonBackColor); // button background
            html.Replace("BTNFORECLR", _buttonForeColor); // button text                        
            html.Replace("BTNHVRCLR", _buttonForeColor); // button text                        
            html.Replace("CDECLR", _codeColor); // button text                        
        }

        private static void BuildFooter(StringBuilder html)
        {
            html.AppendLine("</BODY>");
            html.AppendLine("</HTML>");
        }

        private static void BuildScript(StringBuilder html)
        {
            html.AppendLine("<script>");
            html.AppendLine(GetResource("Scripts.js"));
            html.AppendLine("</script>");
            html.AppendLine("</ HEAD >");
        }

        private static void BuildCss(StringBuilder html)
        {
            html.AppendLine("<style type='text/css'>");
            html.AppendLine(GetResource("Styles.css"));
            html.AppendLine("</style>");
        }
    }
}