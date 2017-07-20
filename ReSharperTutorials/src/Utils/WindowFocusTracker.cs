using EnvDTE;
using JetBrains.DataFlow;

namespace ReSharperTutorials.Utils
{
    internal class WindowFocusTracker
    {
        public IProperty<Window> ActiveWindow;

        public IProperty<bool> IsFocusOnEditor;

        public WindowFocusTracker(Lifetime lifetime)
        {
            var dte = VsIntegration.GetCurrentVsInstance();
            ActiveWindow = new Property<Window>(lifetime, "WindowsFocusTracker.ActiveWindow");
            IsFocusOnEditor = new Property<bool>(lifetime, "WindowsFocusTracker.IsFocusOnEditor")
            {
                Value = dte.ActiveWindow.Document != null
            };

            lifetime.AddBracket(() => dte.Events.WindowEvents.WindowActivated += OnWindowActivated,
                () => dte.Events.WindowEvents.WindowActivated -= OnWindowActivated);
        }

        private void OnWindowActivated(Window gotfocus, Window lostfocus)
        {
            ActiveWindow.Value = gotfocus;
            IsFocusOnEditor.Value = gotfocus.Document != null;
        }
    }
}