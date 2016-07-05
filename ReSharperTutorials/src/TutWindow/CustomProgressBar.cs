using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReSharperTutorials.TutWindow
{
    internal class CustomProgressBar : ProgressBar
    {
        public CustomProgressBar()
        {            
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }                
        
        public string CustomText { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = ClientRectangle;
            var g = e.Graphics;            

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(-1, -1);
            if (Value > 0)
            {                
                var clip = new Rectangle(rect.X, rect.Y, (int) Math.Round((float) Value/Maximum*rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalChunks(g, clip);
            }                                

            using (var f = new Font(FontFamily.GenericSansSerif, 10))
            {
                var len = g.MeasureString(CustomText, f);                
                var location = new Point(Convert.ToInt32(Width/2 - len.Width/2),
                    Convert.ToInt32(Height/2 - len.Height/2));                
                g.DrawString(CustomText, f, Brushes.Black, location);
            }
        }
    }
}

