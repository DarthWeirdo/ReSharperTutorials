using System;
using System.Drawing;
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
        private const string HtmlDoctype = "<!DOCTYPE html>";
        private const string HtmlHead = @"<HTML><HEAD><TITLE></TITLE>";

        private Color _scrollBackColor;
        private Color _scrollFaceColor;
        private Color _scrollArrowColor;
        private Color _disabledTextColor;
        private Color _header1Color;
        private Color _header3Color;
        private Color _mainTextColor;
        private Color _backgroundColor;
        private Color _shortcutBackgroundColor;
        private Color _shortcutBorderColor;
        private Color _shortcutBackgroundDisabledColor;
        private Color _shortcutBorderDisabledColor;


        public HtmlGenerator(Lifetime lifetime, IColorThemeManager colorThemeManager)
        {            
            var isDarkTheme = colorThemeManager.IsDarkTheme(lifetime);
            isDarkTheme.Change.Advise_HasNew(lifetime, args => ApplyColorTheme(args.New ? UiColorTheme.Dark : UiColorTheme.Light));
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
                    throw new FileNotFoundException("Unable to find content. Please reinstall the plugin");
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

        private static string ColorAsHtmlRgb(Color color)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            return $"rgb({r},{g},{b})";
        }

        private void BuildHeader(StringBuilder html)
        {                            
            html.AppendLine(HtmlDoctype);
            html.AppendLine(HtmlHead);
            BuildCss(html);
            BuildScript(html);
            html.AppendLine("<BODY onload=\"stepLoad()\">");
            html.Replace("SCRLFACECLR", ColorAsHtmlRgb(_scrollFaceColor)); // scrollbar tracker 
            html.Replace("SCRLARROWCLR", ColorAsHtmlRgb(_scrollArrowColor)); // scrollbar arrows
            html.Replace("SCRLHLCLR", ColorAsHtmlRgb(_scrollBackColor)); // scrollbar background                                    
            html.Replace("DISFNT", ColorAsHtmlRgb(_disabledTextColor)); // disabled step text color
            html.Replace("H1FNT", ColorAsHtmlRgb(_header1Color)); // heading 1 (step name) color 
            html.Replace("H3FNT", ColorAsHtmlRgb(_header3Color)); // heading 3 (tutorial name on home page) color 
            html.Replace("FNTCLR", ColorAsHtmlRgb(_mainTextColor)); // step text color
            html.Replace("BGCLR", ColorAsHtmlRgb(_backgroundColor)); // window background color
            html.Replace("SHCTBG", ColorAsHtmlRgb(_shortcutBackgroundColor)); // shortcut background color
            html.Replace("SCBGDIS", ColorAsHtmlRgb(_shortcutBackgroundDisabledColor)); // disabled shortcut background color
            html.Replace("SHCTBRD", ColorAsHtmlRgb(_shortcutBorderColor)); // shortcut border color
            html.Replace("SCBRDDIS", ColorAsHtmlRgb(_shortcutBorderDisabledColor)); // disabled shortcut border color

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

        private void ApplyColorTheme(UiColorTheme colorTheme)
        {
            _scrollBackColor = colorTheme.ScrollBackColor;
            _scrollFaceColor = colorTheme.ScrollFaceColor;
            _scrollArrowColor = colorTheme.ScrollArrowColor;
            _header1Color = colorTheme.Header1Color;
            _header3Color = colorTheme.Header3Color;
            _mainTextColor = colorTheme.MainTextColor;
            _backgroundColor = colorTheme.BackgroundColor;
            _disabledTextColor = colorTheme.DisabledTextColor;
            _shortcutBackgroundColor = colorTheme.ShortcutBackgroundColor;
            _shortcutBackgroundDisabledColor = colorTheme.ShortcutBackgroundDisabledColor;
            _shortcutBorderColor = colorTheme.ShortcutBorderColor;
            _shortcutBorderDisabledColor = colorTheme.ShortcutBorderDisabledColor;
        }
    }
}