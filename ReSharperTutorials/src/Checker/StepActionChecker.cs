using System;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using JetBrains.ActionManagement;
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
        private static CommandEvents _commandEvents;
        private readonly IShellLocks _shellLocks;
        private readonly IPsiFiles _psiFiles;
        public string StepActionName;
        public ISignal<bool> AfterActionApplied { get; private set; }        
        public Func<bool> Check = null;


        public StepActionChecker(Lifetime lifetime, IShellLocks shellLocks, IPsiFiles psiFiles, IActionManager actionManager)
        {
            _shellLocks = shellLocks;
            _psiFiles = psiFiles;
            _vsInstance = VsIntegration.GetCurrentVsInstance();
            var events2 = _vsInstance?.Events as Events2;
            if (events2 == null) return;

            _commandEvents = events2.CommandEvents;            

            lifetime.AddBracket(
                () => _commandEvents.AfterExecute += CommandEventsOnAfterExecute, 
                () => _commandEvents.AfterExecute -= CommandEventsOnAfterExecute);

            AfterActionApplied = new Signal<bool>(lifetime, "StepActionChecker.AfterActionApplied");
        }        

        private void CommandEventsOnAfterExecute(string guid, int id1, object customIn, object customOut)
        {            
            if (_vsInstance == null) return;

            var command = _vsInstance.Commands.Item(guid, id1);

//            string logLine = $"Name:{command.Name} | GUID:{command.Guid} | ID:{command.ID}";
//            Log(logLine);

            if (command.Name != StepActionName) return;
            if (Check == null)
                AfterActionApplied.Fire(true);
            else
            {
                _shellLocks.QueueReadLock("StepActionChecker.CheckOnAfterAction",
                  () => _psiFiles.CommitAllDocumentsAsync(CheckCode));                
            }
        }

        private void CheckCode()
        {            
            if (Check())
                AfterActionApplied.Fire(true);
        }

        private void Log(string line)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Log\log.txt", true))
            {
                file.WriteLine(Stopwatch.GetTimestamp() + ": " + line);
            }
        }
    }
}
