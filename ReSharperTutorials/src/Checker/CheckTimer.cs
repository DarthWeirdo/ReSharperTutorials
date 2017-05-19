using System;
using System.Threading;
using JetBrains.DataFlow;

namespace ReSharperTutorials.Checker
{
    internal class CheckTimer
    {
        public ISignal<bool> OnCheckPass { get; }
        public Func<bool> Check = null;

        public CheckTimer(Lifetime lifetime)
        {
            OnCheckPass = new Signal<bool>(lifetime, "CheckTimer.OnTimer");            
            var timer = new Timer(TimerCallback, null, 0, 100);
            lifetime.AddAction(() => timer.Dispose());            
        }

        private void TimerCallback(object state)
        {
            if (Check())
                OnCheckPass.Fire(true);
        }
    }
}
