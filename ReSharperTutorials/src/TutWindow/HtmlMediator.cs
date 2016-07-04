using JetBrains.CommonControls.Browser;
using JetBrains.DataFlow;
using JetBrains.Util;

namespace ReSharperTutorials.TutWindow
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

        public HtmlMediator(Lifetime lifetime, HtmlViewControl viewControl)
        {            
            AllAnimationsDone = new Signal<bool>(lifetime, "HtmlMediator.AllAnimationsDone");
            OnButtonClick = new Signal<bool>(lifetime, "HtmlMediator.OnButtonClick");
            _viewControl = viewControl;
            _viewControl.ObjectForScripting = this;
        }

        public void Animate()
        {
            _viewControl.Document?.InvokeScript("moveOutPrevStep");
        }

        public void MoveOutPrevStepDone()
        {
            MoveOutStepDone = true;            
        }

        public void ClickNextButton()
        {            
            OnButtonClick.Fire(true);            
        }
    }
}
