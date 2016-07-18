using System;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.CommonControls.Browser;
using JetBrains.DataFlow;
using JetBrains.Threading;
using ReSharperTutorials.Runner;

namespace ReSharperTutorials.TutorialUI
{
    /// <summary>
    /// Used to communicate with HTML / JavaScript content in TutorialWindow (e.g., animates step text change, passes button click events)  
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class HtmlMediator
    {
        public ISignal<bool> AllAnimationsDone { get; }
        public ISignal<bool> OnButtonClick { get; }
        private readonly HtmlViewControl _viewControl;
        private bool _moveOutStepDone;
        private IActionManager _actionManager;
        private IShellLocks _shellLocks;
        private Lifetime _lifetime;        
        private IHtmlCommunication _window;

        private bool MoveOutStepDone
        {
            get
            {
                return _moveOutStepDone;
            }
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

        public HtmlMediator(Lifetime lifetime, IHtmlCommunication window, IActionManager actionManager, IShellLocks shellLocks)
        {
            _lifetime = lifetime;
            _window = window;       
            AllAnimationsDone = new Signal<bool>(lifetime, "HtmlMediator.AllAnimationsDone");
            OnButtonClick = new Signal<bool>(lifetime, "HtmlMediator.OnButtonClick");
            _viewControl = window.HtmlViewControl;
            _viewControl.ObjectForScripting = this;
            _actionManager = actionManager;
            _shellLocks = shellLocks;
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
            OnButtonClick.Fire(true);            
        }

        public void RunTutorial(object id)
        {            
            _window.RunTutorial(id.ToString());
        }
    }
}
