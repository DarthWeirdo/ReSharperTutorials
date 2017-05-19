using System;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;

namespace ReSharperTutorials.Checker
{
    internal class StepNavigationChecker
    {
        private readonly IPsiFiles _psiFiles;
        private int _psiTimestamp;

        /// <summary>
        /// This method must check whether a user navigated to the right node. It corresponds
        /// to a method of the MainChecker class and is run on every caret move.
        /// </summary>
        public Func<bool> Check;

        public ISignal<bool> OnCheckPass { get; }

        public StepNavigationChecker(Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
            TextControlManager textControlManager, IShellLocks shellLocks)
        {
            var lifetime1 = lifetime;
            Solution = solution;
            _psiFiles = psiFiles;

            EventHandler caretMoved =
                (sender, args) =>
                {
                    shellLocks.QueueReadLock(lifetime1, "StepNavigationChecker.CheckOnCaretChange", CheckCode);
                };

            lifetime1.AddBracket(
                () => textControlManager.Legacy.CaretMoved += caretMoved,
                () => textControlManager.Legacy.CaretMoved -= caretMoved);

            OnCheckPass = new Signal<bool>(lifetime, "StepNavigationChecker.AfterNavigationDone");
        }

        public ISolution Solution { get; }


        private void CheckCode()
        {
            if (!_psiFiles.AllDocumentsAreCommitted) return;

            if (Check == null) return;
            if (Check())
                OnCheckPass.Fire(true);
        }
    }
}