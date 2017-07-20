using System.Drawing;

namespace ReSharperTutorials.TutorialUI
{
    public class UiColorTheme
    {
        public Color ScrollBackColor { get; private set; }
        public Color ScrollFaceColor { get; private set; }
        public Color ScrollArrowColor { get; private set; }
        public Color Header1Color { get; private set; }
        public Color Header3Color { get; private set; }
        public Color MainTextColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color DisabledTextColor { get; private set; }                
        public Color ShortcutBackgroundColor { get; private set; }
        public Color ShortcutBorderColor { get; private set; }
        public Color ShortcutBackgroundDisabledColor { get; private set; }
        public Color ShortcutBorderDisabledColor { get; private set; }
        public Color MenuItemBackgroundColor { get; private set ; }
        public Color CodeBackgroundColor { get; private set ; }

        public static readonly UiColorTheme Dark = new UiColorTheme
        {
            ScrollBackColor = Color.FromArgb(96, 96, 96),
            ScrollFaceColor = Color.FromArgb(128, 128, 128),
            ScrollArrowColor = Color.FromArgb(160, 160, 160),
            Header1Color = Color.FromArgb(245, 209, 255),
            Header3Color = Color.FromArgb(254, 254, 254),
            MainTextColor = Color.FromArgb(254, 254, 254),
            BackgroundColor = Color.FromArgb(37, 37, 38),
            DisabledTextColor = Color.FromArgb(98, 98, 98),                        
            ShortcutBackgroundColor = Color.FromArgb(101, 92, 105),
            ShortcutBorderColor = Color.FromArgb(140, 128, 145),
            ShortcutBackgroundDisabledColor = Color.FromArgb(68, 68, 68),
            ShortcutBorderDisabledColor = Color.FromArgb(58, 58, 58),
            MenuItemBackgroundColor = Color.FromArgb(60, 60, 64),
            CodeBackgroundColor = Color.FromArgb(69, 85, 99)
        };

        public static readonly UiColorTheme Light = new UiColorTheme
        {
            ScrollBackColor = Color.FromArgb(243, 243, 243),
            ScrollFaceColor = Color.FromArgb(216, 216, 216),
            ScrollArrowColor = Color.FromArgb(127, 127, 127),
            Header1Color = Color.FromArgb(104, 33, 122),
            Header3Color = Color.Black,
            MainTextColor = Color.Black,
            BackgroundColor = Color.White,
            DisabledTextColor = Color.FromArgb(176, 178, 185),                        
            ShortcutBackgroundColor = Color.FromArgb(227, 223, 230),
            ShortcutBorderColor = Color.FromArgb(181, 174, 184),
            ShortcutBackgroundDisabledColor = Color.FromArgb(229, 229, 229),
            ShortcutBorderDisabledColor = Color.FromArgb(184, 184, 184),
            MenuItemBackgroundColor = Color.FromArgb(239,238,243),
            CodeBackgroundColor = Color.FromArgb(224,231,238)
        };
    }
}
