using System.Windows.Forms;
using JetBrains.Annotations;
using JetBrains.UI.Application;
using JetBrains.UI.CommonControls;
using SystemColors = System.Drawing.SystemColors;

namespace ReSharperTutorials.TutorialUI
{
    internal class TutorialPanel : SafePanel
    {
        public TutorialPanel([CanBeNull] IUIApplicationSimple uiapp) : base(uiapp)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.None;
            BackColor = SystemColors.ButtonShadow;
            Padding = new Padding(1, 0, 1, 1);
        }
    }
}