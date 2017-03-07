using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using JetBrains.TextControl.Graphics;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using Point = System.Drawing.Point;
using ProgressBar = System.Windows.Forms.ProgressBar;
using Rectangle = System.Drawing.Rectangle;


namespace ReSharperTutorials.TutorialUI
{
    internal class CustomProgressBar : ProgressBar
    {
        public CustomProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        public string CustomText { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = ClientRectangle;
            var g = e.Graphics;
            var brush = new SolidBrush(Color.FromArgb(122, 193, 255)); // TODO: move to global settings?

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(0, 0);
            if (Value > 0)
            {
                var clip = new Rectangle(rect.X, rect.Y, (int) Math.Round((float) Value / Maximum * rect.Width),
                    rect.Height);
                ProgressBarRenderer.DrawHorizontalBar(g, clip);
                g.FillRectangle(brush, 0, 0, clip.Width, clip.Height);
            }

            using (var f = new Font(FontFamily.GenericSansSerif, 10))
            {
                var len = g.MeasureString(CustomText, f);
                var location = new Point(Convert.ToInt32(Width / 2 - len.Width / 2),
                    Convert.ToInt32(Height / 2 - len.Height / 2));
                g.DrawString(CustomText, f, Brushes.Black, location);
            }
        }
    }
}