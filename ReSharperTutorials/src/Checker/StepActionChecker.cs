using System;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi.Files;
using ReSharperTutorials.Utils;


namespace ReSharperTutorials.Checker
{
    /// <summary>
    /// Used to check whether a user performed an action required by the step.
    /// Action is specified as an argument in XML
    /// </summary>
    internal class StepActionChecker
    {
        private static DTE _vsInstance;
        private readonly IShellLocks _shellLocks;
        private readonly IPsiFiles _psiFiles;
        public string[] StepActionNames;
        public ISignal<bool> OnCheckPass { get; }
        public Func<bool> Check = null;


        public StepActionChecker(Lifetime lifetime, IShellLocks shellLocks, IPsiFiles psiFiles)
        {
            _shellLocks = shellLocks;
            _psiFiles = psiFiles;
            _vsInstance = VsIntegration.GetCurrentVsInstance();
            var events2 = _vsInstance.Events as Events2;
            if (events2 == null) return;

            var commandEvents = events2.CommandEvents;

            lifetime.AddBracket(
                () => commandEvents.AfterExecute += CommandEventsOnAfterExecute,
                () => commandEvents.AfterExecute -= CommandEventsOnAfterExecute);

            OnCheckPass = new Signal<bool>(lifetime, "StepActionChecker.AfterActionApplied");
        }

        private void CommandEventsOnAfterExecute(string guid, int id1, object customIn, object customOut)
        {
            if (_vsInstance == null) return;

            var command = _vsInstance.Commands.Item(guid, id1);

            //string logLine = $"Name:{command.Name} | GUID:{command.Guid} | ID:{command.ID}";
            //Log(logLine);

            foreach (var actionName in StepActionNames)
            {
                if (command.Name != actionName) continue;
                if (Check == null)
                    OnCheckPass.Fire(true);
                else
                {
                    _shellLocks.QueueReadLock("StepActionChecker.CheckOnAfterAction",
                        () => _psiFiles.CommitAllDocumentsAsync(CheckCode));
                }
            }
        }

        private void CheckCode()
        {
            if (Check())
                OnCheckPass.Fire(true);
        }

        private void Log(string line)
        {
            using (var file = new System.IO.StreamWriter(@"C:\Log\log.txt", true))
            {
                file.WriteLine(Stopwatch.GetTimestamp() + ": " + line);
            }
        }
    }
}