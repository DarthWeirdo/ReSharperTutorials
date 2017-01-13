using EnvDTE;
using EnvDTE80;
using JetBrains.DataFlow;
using ReSharperTutorials.TutStep;
using ReSharperTutorials.Utils;

namespace ReSharperTutorials.Checker
{
    public class NextStepPressChecker
    {
        private readonly TutorialStep _step;
        private static DTE _vsInstance;
        private readonly string _nextStepActionName;
//        public ISignal<bool> AfterNextStepActionApplied { get; }


        public NextStepPressChecker(Lifetime lifetime, TutorialStep step, string nextStepActionName)
        {
            _step = step;
            _nextStepActionName = nextStepActionName;
            _vsInstance = VsIntegration.GetCurrentVsInstance();
            var events2 = _vsInstance?.Events as Events2;
            if (events2 == null) return;

            var commandEvents = events2.CommandEvents;

            lifetime.AddBracket(
                () => commandEvents.BeforeExecute += CommandEventsOnBeforeExecute,
                () => commandEvents.BeforeExecute -= CommandEventsOnBeforeExecute);

//            AfterNextStepActionApplied = new Signal<bool>(lifetime, "NextStepPressChecker.AfterNextStepActionApplied");
        }

        private void CommandEventsOnBeforeExecute(string guid, int id, object customin, object customout, ref bool canceldefault)
        {
            if (_vsInstance == null) return;

            var command = _vsInstance.Commands.Item(guid, id);

            if (command.Name == _nextStepActionName)
            {
//            AfterNextStepActionApplied.Fire(true);
                _step.IsCheckDone = true;
                canceldefault = true;
            }
            else
            {
                canceldefault = false;
            }
        }
    }
}