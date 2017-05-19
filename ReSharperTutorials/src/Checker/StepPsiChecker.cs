using System;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperTutorials.Checker
{
    internal class StepPsiChecker
    {
        private readonly IPsiFiles _psiFiles;
        private readonly IShellLocks _shellLocks;
        private int _psiTimestamp;

        /// <summary>
        /// This method checks whether a user have done all the actions 
        /// required by the step by checking the Psi tree. It corresponds
        /// to a method of the MainChecker class and is run on every Psi change.
        /// </summary>
        public Func<bool> Check;

        public ISignal<bool> OnCheckPass { get; }

        public StepPsiChecker(Lifetime lifetime, ISolution solution, IPsiFiles psiFiles, IShellLocks shellLocks)
        {
            Solution = solution;
            _psiFiles = psiFiles;
            _shellLocks = shellLocks;

            Action<ITreeNode, PsiChangedElementType> psiChanged =
                (_, __) => OnPsiChanged();

            lifetime.AddBracket(
                () => psiFiles.AfterPsiChanged += psiChanged,
                () => psiFiles.AfterPsiChanged -= psiChanged);

            OnCheckPass = new Signal<bool>(lifetime, "StepPsiChecker.AfterPsiChangesDone");
        }

        public bool IsUpToDate()
        {
            return _psiTimestamp == Solution.GetPsiServices().Files.PsiTimestamp;
        }

        public ISolution Solution { get; }

        private void OnPsiChanged()
        {
            _shellLocks.QueueReadLock("StepPsiChecker.CheckOnPsiChanged",
                () => _psiFiles.CommitAllDocumentsAsync(CheckCode));
        }

        private void CheckCode()
        {
            if (Check == null) return;
            if (Check())
                OnCheckPass.Fire(true);
        }
    }
}