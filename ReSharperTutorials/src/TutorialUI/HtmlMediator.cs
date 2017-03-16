using JetBrains.CommonControls.Browser;
using JetBrains.DataFlow;
using ReSharperTutorials.Runner;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.TutorialUI
{
    /// <summary>
    /// Used to communicate with HTML / JavaScript content in TutorialWindow (e.g., animates step text change, passes button click events)  
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class HtmlMediator
    {
        public ISignal<bool> AllAnimationsDone { get; }
        public ISignal<bool> OnNextStepButtonClick { get; }
        public ISignal<bool> OnRunStepNavigationLinkClick { get; }
        public ISignal<bool> OnPageHasFullyLoaded { get; }
        private readonly HtmlViewControl _viewControl;
        private bool _moveOutStepDone;
        private IHtmlCommunication _window;

        private bool MoveOutStepDone
        {
            get { return _moveOutStepDone; }
            set
            {
                _moveOutStepDone = value;
//                if (IsOtherAnimationsDone)                
                OnAnimationsDone();
            }
        }

        private void OnAnimationsDone()
        {
            AllAnimationsDone.Fire(true);
        }

        public HtmlMediator(Lifetime lifetime, IHtmlCommunication window)
        {
            _window = window;
            AllAnimationsDone = new Signal<bool>(lifetime, "HtmlMediator.AllAnimationsDone");
            OnNextStepButtonClick = new Signal<bool>(lifetime, "HtmlMediator.OnButtonClick");
            OnRunStepNavigationLinkClick = new Signal<bool>(lifetime, "HtmlMediator.OnRunStepNavigationLinkClick");
            OnPageHasFullyLoaded = new Signal<bool>(lifetime, "HtmlMediator.OnPageHasFullyLoaded");
            _viewControl = window.HtmlViewControl;
            _viewControl.ObjectForScripting = this;
        }

        public void Animate()
        {
            _viewControl.Document?.InvokeScript("moveOutPrevStep");
        }

        public void EnableButtons(bool state)
        {
            _viewControl.Document?.InvokeScript(state ? "enableButtons" : "disableButtons");
        }

        public void AgreeToRunTutorial(string htmlTutorialId, string imgSrc)
        {
            var objArray = new object[2];
            objArray[0] = htmlTutorialId;
            objArray[1] = imgSrc;
//            _viewControl.Document?.InvokeScript("agreeToRunTutorial", objArray);            
            _viewControl.Document?.InvokeScript("agreeToRunTutorial");
        }

        public void HideImages()
        {
            _viewControl.Document?.InvokeScript("hideImages");
        }

        public void MoveOutPrevStepDone()
        {
            MoveOutStepDone = true;
        }

        public void ClickNextButton()
        {
            OnNextStepButtonClick.Fire(true);
        }

        public void RunTutorial(object id)
        {
            _window.RunTutorial(id.ToString());
        }

        public void OpenLink(object link)
        {
            System.Diagnostics.Process.Start(link.ToString());
        }

        public void RunStepNavigation()
        {
            OnRunStepNavigationLinkClick.Fire(true);
        }

        public void CloseSolution()
        {
            VsIntegration.CloseVsSolution(true);
        }

        public void ChangeNextStepButtonText(bool isFocusOnEditor)
        {
            var nextStepShortcut = VsIntegration.GetActionShortcut(GlobalSettings.NextStepShortcutAction);
            var args = new object[] {isFocusOnEditor, nextStepShortcut};
            _viewControl.Document?.InvokeScript("setNextStepButtonText", args);
        }

        public void PageHasFullyLoaded()
        {
            OnPageHasFullyLoaded.Fire(true);
        }
    }
}