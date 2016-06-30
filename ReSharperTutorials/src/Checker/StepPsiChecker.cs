using System;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.Application;

namespace ReSharperTutorials.Checker
{
    internal class StepPsiChecker
    {

        private readonly Lifetime _lifetime;
        private readonly ISolution _solution;
        private readonly IPsiFiles _psiFiles;
        private readonly TextControlManager _textControlManager;
        private readonly IShellLocks _shellLocks;
        private readonly IEditorManager _editorManager;
        private readonly DocumentManager _documentManager;
        private readonly IUIApplication _environment;
        private int _psiTimestamp;

        /// <summary>
        /// This method checks whether a user have done all the actions 
        /// required by the step by checking the Psi tree. It corresponds
        /// to a method of the MainChecker class and is run on every Psi change.
        /// </summary>
        public Func<bool> Check;

        public ISignal<bool> AfterPsiChangesDone { get; private set; }

        public StepPsiChecker(Lifetime lifetime, ISolution solution, IPsiFiles psiFiles,
                                  TextControlManager textControlManager, IShellLocks shellLocks,
                                  IEditorManager editorManager, DocumentManager documentManager, IUIApplication environment)
        {
            _lifetime = lifetime;
            _solution = solution;
            _psiFiles = psiFiles;
            _textControlManager = textControlManager;
            _shellLocks = shellLocks;
            _documentManager = documentManager;
            _environment = environment;
            _editorManager = editorManager;

            Action<ITreeNode, PsiChangedElementType> psiChanged =
                (_, __) => OnPsiChanged();

            _lifetime.AddBracket(
              () => psiFiles.AfterPsiChanged += psiChanged,
              () => psiFiles.AfterPsiChanged -= psiChanged);

            AfterPsiChangesDone = new Signal<bool>(lifetime, "StepPsiChecker.AfterPsiChangesDone");
        }

        public bool IsUpToDate()
        {
            // PsiTimestamp is a time stamp for PsiFiles - use it to check the current PSI is up to date
            return _psiTimestamp == Solution.GetPsiServices().Files.PsiTimestamp;
        }

        public ISolution Solution
        {
            get { return _solution; }
        }

        private void OnPsiChanged()
        {
            _shellLocks.QueueReadLock("StepPsiChecker.CheckOnPsiChanged",
                  () => _psiFiles.CommitAllDocumentsAsync(CheckCode));
        }

        private void CheckCode()
        {
            if (Check == null) return;
            if (Check())
                AfterPsiChangesDone.Fire(true);
        }
    }
}
